using _1СDataServiceSQL.DAL.DTModels;
using Dapper;
using Siemens.SimaticIT.ReferenceData.UDM_RF.RFModel.Types.ReadingModel;
using Siemens.SimaticIT.Runtime.Common;
using Siemens.SimaticIT.U4DM.AppU4DM.AppU4DM.DMPOMModel.Commands.Published;
using Siemens.SimaticIT.U4DM.AppU4DM.AppU4DM.DMPOMModel.DataModel.ReadingModel;
using Siemens.SimaticIT.Unified.Lean;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMCDataService.DAL.Interfaces;
using UMCDataService.DAL.Repositories;

namespace APSDataService.DAL
{
    public class WorkOrder1CSQLRepository : IRepositorySQL1C<WorkOrder>,IDisposable
    {
        private string _connectionString;
        private dynamic connect;
        public int progressStatus;
        public int progressMaxStatus;

        CreateWorkOrder.Response response;
        string responseStr;
        DM_MaterialRepository dm_MatRepo;
        MaterialGroupRepository dm_MatClassesRepo;
        MaterialRepository matRepo;
        WorkOrderRepository WorkOrderRepo;
        WorkOrderOperationRepository WorkOrderOperationRepo;

        OperationRepository OperationRepo;
        UoMRepository UoMRepo;
        UoM uom;
        ToBeConsumedMaterialRepository tcmRepo;
        IUnifiedSdkLean unifiedSdkLean;

        public void initSdkLean()
        {
            LeanFactory.Initialize("GatewayUOWApp");
            unifiedSdkLean = LeanFactory.Create();
            unifiedSdkLean.SetWindowsAuthentication();
        }

        public void InitRepositories()
        {
            dm_MatClassesRepo = new MaterialGroupRepository(unifiedSdkLean);
            dm_MatRepo = new DM_MaterialRepository(unifiedSdkLean);
            matRepo = new MaterialRepository(unifiedSdkLean);
            WorkOrderRepo = new WorkOrderRepository(unifiedSdkLean);
            WorkOrderOperationRepo = new WorkOrderOperationRepository(unifiedSdkLean);
            OperationRepo = new OperationRepository(unifiedSdkLean);
            UoMRepo = new UoMRepository(unifiedSdkLean);
            tcmRepo = new ToBeConsumedMaterialRepository(unifiedSdkLean);

        }



        public WorkOrder1CSQLRepository(IUnifiedSdkLean unifiedSdkLean = null)
        {
            this.unifiedSdkLean = unifiedSdkLean;
            _connectionString = @"Data Source=DMKIM\MSSQLSERVER1;Integrated Security=True;Initial Catalog=PBD_Preactor;";
            if (unifiedSdkLean == null)
            {
                initSdkLean();
            }
            InitRepositories();
        }

        public bool Create(WorkOrder obj)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }
        public void CreateDM_Material(string dmMatElKod,string dmMatElName)
        {
            var matNewInstance = new Material
            {
                Name = dmMatElName,
                NId = dmMatElKod,
                Revision ="A",
                UoMNId = UoMRepo.GetByName("Unit").NId
            };
            matRepo.Create(matNewInstance);
            matNewInstance = matRepo.GetById(dmMatElKod);//на деле NId
            var dmNewMat = new DM_Material
                {
                    Material = matNewInstance,
                    LogisticClassNId = "Default",
                    MaterialClass_Id = dm_MatClassesRepo.GetByName("n/a").Id,
                    Material_Id = matNewInstance.Id
            };
            dm_MatRepo.Create(dmNewMat);
        }


        public void CreateWorkOrder(string orderNo, string partNo,decimal qty,DateTime releaseDate, DateTime dueDate)
        {
            var finMat = dm_MatRepo.GetByNId(partNo);

            var newWorkOrderEl = new WorkOrder
            {
                NId = orderNo,
                Name = orderNo,
                FinalMaterial_Id = finMat?.Id,
                InitialQuantity = qty,
                ProducedQuantity = qty,
                CreationDate = releaseDate,
                DueDate = dueDate,
                WorkOrderOperations = new List<WorkOrderOperation>(),
                Plant = "Enterprise"
            };
            WorkOrderRepo.Create(newWorkOrderEl);
        }

        public void AddToBeConsumedMaterial( Guid? woOpId, string woOpNId, string requiredPartNo, decimal requiredPartQuantity)
        {
            var reqMatItem = dm_MatRepo.GetByNId(requiredPartNo);
            var foundMatItem = matRepo.GetById(requiredPartNo);
            if (foundMatItem == null || reqMatItem == null) return;
            var tbcMatITem = new ToBeConsumedMaterial
            {
                NId = woOpId + "-" + foundMatItem.NId,
                Name = foundMatItem.Name,
                Quantity = requiredPartQuantity,
                WorkOrderOperation_Id = woOpId,
                MaterialDefinition_Id = reqMatItem.Id,
            };
            tcmRepo.Create(tbcMatITem);
        }

        public void CreateWorkOrderOperation(string operationNo, string operationName, DateTime releaseDate,Guid? woId)
        {
            var wItemNew = new WorkOrderOperation
            {
                Name = operationName,
                NId = operationNo,
                Description = "1C operation:" + operationNo,
                CreatedOn = releaseDate,
                WorkOrder_Id = woId,
                ToBeConsumedMaterials = new List<ToBeConsumedMaterial>()
            };
            WorkOrderOperationRepo.Create(wItemNew);

        }

        public void CreateOperation(string operationNo, string operationName, DateTime releaseDate, Guid? woId)
        {
            var wItemNew = new Operation
            {
                Name = operationName,
                NId = operationNo,
                Description = "1C operation:" + operationNo,
                CreatedOn = releaseDate,
                WorkOperationId_Id = woId,
            };
            OperationRepo.Create(wItemNew);
        }

        public IEnumerable<WorkOrder> GetAllWithCreation()
        {
            IEnumerable<QueryWorkOrderSql> list;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "p_ImportFOBD_FromPBD_ToMES";
                connection.Open();
                list = connection.Query<QueryWorkOrderSql>(sql, commandType: CommandType.StoredProcedure);
            };

            var resultList = new List<WorkOrder>();
            var elOrderNoOld = "-1";
            WorkOrder newWorkOrderEl = null;
            WorkOrderOperation wItemNew = null;

            foreach (var el in list)
            {
                var orderNo = el.OrderNo;
                var partNo = el.PartNo;
                var quantity = el.Quantity;
                var requiredPartNo = el.RequiredPartNo;
                var requiredQuantity = el.RequiredQuantity;
                var BD = el.BD;
                var UID = el.UID;
                var ignoreShortages = el.IgnoreShortages;
                var multipleQuantity = el.MultipleQuantity;
                var multiply = el.Multiply;
                var operationName = el.OperationName;
                var operationNo = el.OperationNo;
                var releaseDate = el.ReleaseDate;
                var dueDate = el.DueDate;

                if (orderNo != elOrderNoOld)
                {
                    if (newWorkOrderEl != null)
                        resultList.Add(newWorkOrderEl);

                    var woEl = WorkOrderRepo.GetById(orderNo);//на деле NId
                    if (woEl == null)
                    {
                        var finMat = dm_MatRepo.GetByNId(partNo);
                        if (finMat==null)
                        {
                            CreateDM_Material(partNo, partNo);//!!!! нет наименования.предполагается что уже закачаны
                            finMat = dm_MatRepo.GetByNId(partNo);
                        }

                        CreateWorkOrder(orderNo, partNo, quantity, releaseDate, dueDate);
                        woEl = WorkOrderRepo.GetById(orderNo);
                    }
                    newWorkOrderEl = woEl;
                }
                else
                {
                }

                var operationNId = orderNo + "-" + operationNo;
                var woOpElItem = WorkOrderOperationRepo.GetById(operationNId);
                if (woOpElItem == null)
                {
                    CreateWorkOrderOperation(operationNId, operationName, releaseDate, newWorkOrderEl.Id);
                    woOpElItem = WorkOrderOperationRepo.GetById(operationNId);
                }

                var tcmItem = tcmRepo.GetById(requiredPartNo, woOpElItem.Id);
                if (tcmItem==null)
                {
                    AddToBeConsumedMaterial(woOpElItem.Id, woOpElItem.NId, requiredPartNo, requiredQuantity);
                    //tcmItem = tcmRepo.GetById(requiredPartNo);
                }

                newWorkOrderEl.WorkOrderOperations.Add(woOpElItem);
                elOrderNoOld = orderNo;
            }
            return resultList;
        }

        public async Task<IEnumerable<WorkOrder>> GetAllAsync()
        {
            return await Task.Run(() =>
            {
                return GetAll();

            });

        }

        public IEnumerable<WorkOrder> GetAll()
        {
            IEnumerable<QueryWorkOrderSql> list;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "p_ImportFOBD_FromPBD_ToMES";
                connection.Open();
                //list = connection.Query<QueryWorkOrderSql>(sql, new { BD = 1 }, commandType: CommandType.StoredProcedure);
                list = connection.Query<QueryWorkOrderSql>(sql,  commandType: CommandType.StoredProcedure);
            };


            var resultList = new List<WorkOrder>();
            progressStatus = 0;
            progressMaxStatus = list.Count();

            var elOrderNoOld = "-1";
            WorkOrder newWorkOrderEl = null;
            WorkOrderOperation wItemNew = null;

            foreach (var el in list)
            {
                progressStatus++;

                var orderNo = el.OrderNo;
                var partNo = el.PartNo;
                var quantity = el.Quantity;
                var requiredPartNo = el.RequiredPartNo;
                var requiredQuantity = el.RequiredQuantity;
                var BD = el.BD;
                var UID = el.UID;
                var ignoreShortages = el.IgnoreShortages;
                var multipleQuantity = el.MultipleQuantity;
                var multiply = el.Multiply;
                var operationName = el.OperationName;
                var operationNo = el.OperationNo;
                var releaseDate = el.ReleaseDate;
                var dueDate = el.DueDate;

                if (orderNo != elOrderNoOld)
                {
                    if (newWorkOrderEl != null)
                        resultList.Add(newWorkOrderEl);

                    newWorkOrderEl = new WorkOrder
                    {
                        NId = orderNo,
                        Name = orderNo,
                        FinalMaterial_Id = dm_MatRepo.GetByNId(partNo)?.Id,
                        FinalMaterial = dm_MatRepo.GetByNId(partNo),
                        InitialQuantity = el.Quantity,
                        ProducedQuantity = el.Quantity,
                        CreationDate = el.ReleaseDate,
                        DueDate = el.DueDate,
                        WorkOrderOperations = new List<WorkOrderOperation>()
                    };

                }
                else
                {
                }
                wItemNew = new WorkOrderOperation
                {
                    Name = operationName,
                    NId =operationNo,
                    Description = "1C operation:" + operationNo,
                    CreatedOn = releaseDate,
                    ToBeConsumedMaterials = new List<ToBeConsumedMaterial>()

                };

                var reqMatItem = dm_MatRepo.GetByNId(requiredPartNo);
                var foundMatItem = matRepo.GetById(requiredPartNo);
                if (foundMatItem != null || reqMatItem != null)
                {
                    var tbcMatITem = new ToBeConsumedMaterial
                    {
                        NId = operationNo + "-" + foundMatItem.NId,
                        Name = foundMatItem.Name,
                        Quantity = requiredQuantity,
                        MaterialDefinition_Id = reqMatItem.Id,
                        MaterialDefinition = reqMatItem
                    };
                    wItemNew.ToBeConsumedMaterials.Add(tbcMatITem);
                }


                newWorkOrderEl.WorkOrderOperations.Add(wItemNew);
                elOrderNoOld = orderNo;
            }

            return resultList;
            

        }

        public IEnumerable<WorkOrder> GetAllByDateInterval(DateTime dateFrom, TimeSpan dateOffset)
        {
            throw new NotImplementedException();
        }


        public WorkOrder GetById(string id)
        {
            throw new NotImplementedException();
        }

        public void Update(WorkOrder obj)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            try
            {
                //LeanFactory.ShutDown();
            }
            catch
            {

            }
        }

        ~WorkOrder1CSQLRepository()
        {
            Dispose();

        }


    }
}

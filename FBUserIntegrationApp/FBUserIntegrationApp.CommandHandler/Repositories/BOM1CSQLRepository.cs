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
    public class BoM1CSQLRepository : IRepositorySQL1C<BoM>,IDisposable
    {
        private string _connectionString;
        private dynamic connect;
        public int progressStatus;
        public int progressMaxStatus;

        CreateWorkOrder.Response response;
        string responseStr;
        IUnifiedSdkLean unifiedSdkLean;
        DM_MaterialRepository dm_MatRepo;
        MaterialGroupRepository dm_MatClassesRepo;
        MaterialRepository matRepo;
        BoMRepository BoMRepo;
        BoMItemRepository BoMItemRepo;
        UoMRepository UoMRepo;
        UoM uom;

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
            BoMRepo = new BoMRepository(unifiedSdkLean);
            UoMRepo = new UoMRepository(unifiedSdkLean);
            BoMItemRepo = new BoMItemRepository(unifiedSdkLean);
        }



        public BoM1CSQLRepository(IUnifiedSdkLean unifiedSdkLean = null)
        {
            this.unifiedSdkLean = unifiedSdkLean;
            _connectionString = @"Data Source=DMKIM\MSSQLSERVER1;Integrated Security=True;Initial Catalog=PBD_Preactor;";
            if (unifiedSdkLean == null)
            {
                initSdkLean();
            }
            InitRepositories();
        }

        public bool Create(BoM obj)
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


        public void CreateBoM(string dmMatElKod, string dmMatElName,decimal? qty,UoM uom,DM_Material dmMatEl)
        {
            var newBoMEl = new BoM
            {
                Description = dmMatElName,
                Name = dmMatElName,
                NId = dmMatElKod,
                Quantity = new QuantityType { QuantityValue = qty,UoMNId = uom?.NId},
                BoMItems = new List<BoMItem>(),
                MaterialDefinition = dmMatEl,
                MaterialDefinition_Id = dmMatEl.Id,
                Version = "A"
            };
            BoMRepo.Create(newBoMEl);
        }

        public void CreateBoMItem(string dmMatElKodItem, string dmMatElNameItem, decimal? qty,UoM uom, BoM newBoMEl, DM_Material dmMatElItem)
        {
            BoMItem bItemNew = new BoMItem
            {
                BoM_Id = newBoMEl?.Id,
                BoM = newBoMEl,
                NId = dmMatElKodItem,
                Quantity = new QuantityType {QuantityValue = qty,UoMNId = uom?.NId },
                MaterialDefinition = dmMatElItem
            };
            BoMItemRepo.Create(bItemNew);
        }
        
        public IEnumerable<BoM> GetAllWithCreation()
        {
            IEnumerable<QueryBomSql> list;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "p_ImportBOM_FromPBD_ToMES";
                connection.Open();
                list = connection.Query<QueryBomSql>(sql, new { BD = 1 }, commandType: CommandType.StoredProcedure);
            };
            var resultList = new List<BoM>();
            var elProductCodeOld = "-1";
            BoM newBoMEl = null;
            DM_MaterialRepository dm = new DM_MaterialRepository(unifiedSdkLean);

            foreach (var el in list)
            {
                uom = UoMRepo.GetByName("Unit");
                var dmMatElKod = el.PartNo;
                var dmMatElItemKod = el.RequiredPartNo;
                var dmMatElItemName = el.RequiredPartName;
                var dmMatElName = el.PartName;
                decimal qtyBoMItemParse = 0;
                decimal.TryParse(el.RequiredQuantity, out qtyBoMItemParse);
                var qtyBoM = new QuantityType { QuantityValue = (decimal?)1, UoMNId= uom.NId };
                var qtyBoMItem = new QuantityType { QuantityValue = (decimal?)qtyBoMItemParse, UoMNId = uom.NId };
                var dmMatElItem = dm_MatRepo.GetByNId(dmMatElItemKod);
                if (dmMatElItem == null)
                {
                    CreateDM_Material(dmMatElItemKod, dmMatElItemName);
                    dmMatElItem = dm_MatRepo.GetByNId(dmMatElItemKod);
                }
                if (dmMatElKod != elProductCodeOld)
                {
                    if (newBoMEl != null)
                    {
                        resultList.Add(newBoMEl);
                    }
                    var dmMatEl = dm_MatRepo.GetByNId(dmMatElKod);
                    if (dmMatEl == null)
                    {
                        CreateDM_Material(dmMatElKod, dmMatElName);
                        dmMatEl = dm_MatRepo.GetByNId(dmMatElKod);
                    }
                    var BoMEl = BoMRepo.GetById(dmMatElKod);//на деле NId
                    if (BoMEl == null)
                    {
                        CreateBoM(dmMatElKod, dmMatElName, qtyBoM.QuantityValue,uom, dmMatEl);
                        BoMEl = BoMRepo.GetById(dmMatElKod);//на деле NId
                    }
                    newBoMEl = BoMEl;

//                    var str = dmMatElKod + " |" + dmMatElName+ "|"+el.PartNo + " |" + el.BD + " |"+ 1;
                    //Console.WriteLine(str);
                }
                else
                {
                    if (dmMatElItem!=null)
                    {
                        //Console.WriteLine(str);
                    }
                }

                var BoMlItem = (BoMItem)BoMItemRepo.GetById(dmMatElItemKod);
                if (BoMlItem == null)
                {
                    CreateBoMItem(dmMatElItemKod, dmMatElItemName, qtyBoMItem.QuantityValue, uom, newBoMEl, dmMatElItem);
                    BoMlItem = BoMItemRepo.GetById(dmMatElItemKod);
                }
                newBoMEl.BoMItems.Add(BoMlItem);

                elProductCodeOld = dmMatElKod;

            }

            return resultList;


        }

        public async Task<IEnumerable<BoM>> GetAllAsync()
        {
            return await Task.Run(() =>
            {
                return GetAll();

            });

        }



        public IEnumerable<BoM> GetAll()
        {
            IEnumerable<QueryBomSql> list;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "p_ImportBOM_FromPBD_ToMES";
                connection.Open();
                list = connection.Query<QueryBomSql>(sql,new { BD=1 },commandType: CommandType.StoredProcedure);
            };


            var resultList = new List<BoM>();
            progressStatus = 0;
            progressMaxStatus = list.Count();

            var elProductCodeOld = "-1";
            BoM newBoMEl = null;
            BoMItem bItemNew = null;
            DM_MaterialRepository dm = new DM_MaterialRepository(unifiedSdkLean);
            uom = UoMRepo.GetByName("Unit");

            foreach (var el in list)
            {
                progressStatus++;

                var dmMatElKod = el.PartNo;
                var dmMatElItemKod = el.RequiredPartNo;
                var dmMatElItemName = el.RequiredPartName;
                var dmMatElName = el.PartName;
                var dmMatEl = dm.GetByNId(dmMatElKod);
                var dmMatElItem = dm.GetByNId(dmMatElItemKod);
                decimal qtyBoMItem = 0;
                decimal.TryParse(el.RequiredQuantity, out qtyBoMItem);

                if (dmMatElKod != elProductCodeOld)
                {
                    if (newBoMEl != null)
                        resultList.Add(newBoMEl);

                    if (dmMatEl == null)
                    {
                        dmMatEl = new DM_Material
                        {
                            Material = new Material
                            {
                                Name = dmMatElName,
                                NId = dmMatElKod

                            }
                        };
                    }
                    newBoMEl = new BoM
                    {
                        Description = dmMatElName,
                        Name = dmMatElName,
                        NId = dmMatElKod,
                        Quantity = new QuantityType { QuantityValue = (decimal?)1, UoMNId=uom.NId },
                        BoMItems = new List<BoMItem>(),
                        MaterialDefinition = dmMatEl
                    };

                    var str = "****"+dmMatElKod + " |" + dmMatElName + "|" + el.PartNo + " |" + el.BD + " |" + el.RequiredQuantity;
                    //Console.WriteLine(str);
                }
                else
                {
                    /*
                    var str =
                    el.OperationName + " |" +
                    el.OperationNo + " |" +
                    el.Multiply + " |" +
                    el.IgnoreShortages + " |" +
                    el.MultipleQuantity + " |" +
                    el.RequiredPartName + " |" +
                    el.RequiredPartNo + " |" +
                    el.RequiredQuantity + " |";
                    //Console.WriteLine(str);
                    */
                }

                if (dmMatElItem == null)
                {
                    dmMatElItem = new DM_Material
                    {
                        Material = new Material
                        {
                            Name = dmMatElItemName,
                            NId = dmMatElItemKod
                            
                        }
                    };
                }


                bItemNew = new BoMItem
                {
                    BoM_Id = (Guid?)newBoMEl?.Id,
                    BoM = newBoMEl,
                    NId = dmMatElItemKod,
                    Quantity = new QuantityType {QuantityValue = (Decimal?)qtyBoMItem, UoMNId = uom.NId },
                    MaterialDefinition = dmMatElItem
                    
                };
                newBoMEl.BoMItems.Add(bItemNew);

                elProductCodeOld = dmMatElKod;


            }

            return resultList;
            

        }

        public IEnumerable<BoM> GetAllByDateInterval(DateTime dateFrom, TimeSpan dateOffset)
        {
            throw new NotImplementedException();
        }


        public BoM GetById(string id)
        {
            throw new NotImplementedException();
        }

        public void Update(BoM obj)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            //LeanFactory.ShutDown();
        }

        public void DisposeRepositories()
        {
            //unifiedSdkLean.Dispose();
        }



        ~BoM1CSQLRepository()
        {
            DisposeRepositories();
            Dispose();

        }


    }
}

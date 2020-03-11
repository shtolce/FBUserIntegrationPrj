using _1СDataServiceSQL.DAL.DTModels;
using Dapper;
using IntegrationApp.FBUserIntegrationApp.FBUserIntegrationApp.IAPOMModel.DataModel.ReadingModel;
using Siemens.SimaticIT.ReferenceData.UDM_RF.RFModel.Types.ReadingModel;
using Siemens.SimaticIT.Unified;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMCDataService.DAL.Interfaces;

namespace IntegrationLibrary.MasterData.FBIntegrationLibrary.MSModel.DAL
{
    public class BoM1CSQLRepository : IRepositorySQL1C<BoM>,IDisposable
    {
        private string _connectionString;
        private dynamic connect;
        public int progressStatus;
        public int progressMaxStatus;

        string responseStr;
        UoM uom;

        public BoM1CSQLRepository()
        {
            _connectionString = @"Data Source=DMKIM\MSSQLSERVER1;Integrated Security=True;Initial Catalog=PBD_Opcenter;";
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
                Revision ="A"
            };
            var dmNewMat = new DM_Material
                {
                    Material = matNewInstance,
                    LogisticClassNId = "Default"
            };
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
            var elProductCodeOld = "-1";
            BoM newBoMEl = null;
            BoMItem bItemNew = null;

            foreach (var el in list)
            {
                progressStatus++;

                var dmMatElKod = el.PartNo;
                var dmMatElItemKod = el.RequiredPartNo;
                var dmMatElItemName = el.RequiredPartName;
                var dmMatElName = el.PartName;
                DM_Material dmMatEl = new DM_Material();
                DM_Material dmMatElItem = new DM_Material();
                decimal qtyBoMItem = 0;
                decimal.TryParse(el.RequiredQuantity, out qtyBoMItem);

                if (dmMatElKod != elProductCodeOld)
                {
                    if (newBoMEl != null)
                        resultList.Add(newBoMEl);

                    dmMatEl = new DM_Material
                    {
                        Material = new Material
                        {
                            Name = dmMatElName,
                            NId = dmMatElKod,
                            Revision = "A"
                        }
                    };
                    newBoMEl = new BoM
                    {
                        Description = dmMatElName,
                        Name = dmMatElName,
                        NId = dmMatElKod,
                        Quantity = new QuantityType { QuantityValue = (decimal?)1, UoMNId=uom?.NId },
                        BoMItems = new List<BoMItem>(),
                        MaterialDefinition = dmMatEl,
                        Version = "A"

                    };
                }
// Description,Name,NId,Quantity,BoMItems,MaterialDefinition,Version ,BoMItem_NId,BoMItem_Quantity,BoMItem_MaterialDefinition, BoMItem_Name, BoMItem_NId



                else
                {
                }

                dmMatElItem = new DM_Material
                {
                    Material = new Material
                    {
                        Name = dmMatElItemName,
                        NId = dmMatElItemKod
                            
                    }
                };

                bItemNew = new BoMItem
                {
                    BoM_Id = (Guid?)newBoMEl?.Id,
                    BoM = newBoMEl,
                    NId = dmMatElItemKod,
                    Quantity = new QuantityType {QuantityValue = (Decimal?)qtyBoMItem, UoMNId = uom?.NId },
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

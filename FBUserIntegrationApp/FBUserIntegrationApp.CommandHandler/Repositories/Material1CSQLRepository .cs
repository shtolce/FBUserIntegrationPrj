using _1СDataServiceSQL.DAL.DTModels;
using Dapper;
using IntegrationApp.FBUserIntegrationApp.FBUserIntegrationApp.IAPOMModel.DataModel.ReadingModel;
using IntegrationLibrary.MasterData.FBIntegrationLibrary.MSModel.DAL;
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

namespace APSDataService.DAL
{
    public class Material1CSQLRepository : IRepositorySQL1C<DM_Material>
    {
        private string _connectionString = @"Data Source=DMKIM\MSSQLSERVER1;Integrated Security=True;Initial Catalog=PBD_Opcenter;";
        private dynamic connect;
        public int progressStatus;
        public int progressMaxStatus;

        string responseStr;
        DM_MaterialRepository dm_MatRepo;
        MaterialGroupRepository dm_MatClassesRepo;
        MaterialRepository matRepo;
        UoMRepository UoMRepo;
        UoM uom;

        public void InitRepositories()
        {
            //dm_MatClassesRepo = new MaterialGroupRepository(platform);
            //dm_MatRepo = new DM_MaterialRepository(platform);
            //matRepo = new MaterialRepository(platform);
            //UoMRepo = new UoMRepository(platform);
        }

        public Material1CSQLRepository()
        {
            _connectionString = @"Data Source=DMKIM\MSSQLSERVER1;Integrated Security=True;Initial Catalog=PBD_Opcenter;";
            InitRepositories();
        }

        public bool Create(DM_Material obj)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<DM_Material>> GetAllAsync()
        {
            return await Task.Run(() =>
            {
                return GetAll();
            });
        }

        public IEnumerable<DM_Material> GetAll()
        {
            IEnumerable<QueryMaterialSql> list;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "p_ImportMaterial_FromPBD_ToMES";
                connection.Open();
                list = connection.Query<QueryMaterialSql>(sql, new { BD = 1 }, commandType: CommandType.StoredProcedure);
            };
            var resultList = new List<DM_Material>();

            foreach (var el in list)
            {
                progressStatus++;
                uom = UoMRepo.GetByName("Unit");
                var dmMatElKod = el.PartNo;
                var dmMatElName = el.Product;
                var dmMatEl = dm_MatRepo.GetByNId(dmMatElKod);
                if (dmMatEl == null)
                {
                    var matNewInstance = new Material
                    {
                        Name = dmMatElName,
                        NId = dmMatElKod,
                        Revision = "A",
                    };
                    var dmNewMat = new DM_Material
                    {
                        Material = matNewInstance,
                        LogisticClassNId = "Default",
                    };

                    dmMatEl = dmNewMat;
                }
                resultList.Add(dmMatEl);
            }
            return resultList;
        }

        public IEnumerable<DM_Material> GetAllByDateInterval(DateTime dateFrom, TimeSpan dateOffset)
        {
            throw new NotImplementedException();
        }


        public DM_Material GetById(string id)
        {
            throw new NotImplementedException();
        }

        public void Update(DM_Material obj)
        {
            throw new NotImplementedException();
        }


    }
}

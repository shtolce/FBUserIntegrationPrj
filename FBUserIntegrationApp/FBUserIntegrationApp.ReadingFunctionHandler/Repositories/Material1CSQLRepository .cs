using _1СDataServiceSQL.DAL.DTModels;
using Dapper;
using IntegrationApp.FBUserIntegrationApp.FBUserIntegrationApp.IAPOMModel.DataModel.ReadingModel;
using Siemens.SimaticIT.ReferenceData.UDM_RF.RFModel.Types.ReadingModel;
using Siemens.SimaticIT.Unified;
using Siemens.SimaticIT.Unified.Common;
using Siemens.SimaticIT.Unified.Lean;
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
    public class Material1CSQLRepository 
    {
        private string _connectionString;
        private dynamic connect;
        public int progressStatus;
        public int progressMaxStatus;

        string responseStr;
        UoM uom;

        IQueryable<Equipment> EquipmentDataSource;
        public Material1CSQLRepository()
        {
            _connectionString = @"Data Source=DMKIM\MSSQLSERVER1;Integrated Security=True;Initial Catalog=PBD_Opcenter;";
        }

        public bool Create(DM_Material obj)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
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
            progressStatus = 0;
            progressMaxStatus = list.Count();
            DM_Material dmMatEl;
            foreach (var el in list)
            {
                progressStatus++;
                var dmMatElKod = el.PartNo;
                var dmMatElName = el.Product;

                var matNewInstance = new Material
                {
                    Name = dmMatElName,
                    NId = dmMatElKod,
                    Revision = "A"
                };
                var dmNewMat = new DM_Material
                {
                    Material = matNewInstance,
                    LogisticClassNId = "Default"
                };

                dmMatEl = dmNewMat;
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

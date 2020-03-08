using _1СDataServiceSQL.DAL.DTModels;
using Dapper;
using IntegrationApp.FBUserIntegrationApp.FBUserIntegrationApp.IAPOMModel.DataModel.ReadingModel;
//using Siemens.SimaticIT.U4DM.AppU4DM.AppU4DM.DMPOMModel.DataModel.ReadingModel;
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
    public class Equipment1CSQLRepository : IRepositorySQL1C<Equipment>,IDisposable
    {
        private string _connectionString = @"Data Source=DMKIM\MSSQLSERVER1;Integrated Security=True;Initial Catalog=PBD_Opcenter;";
        public int progressStatus;
        public int progressMaxStatus;

        public Equipment1CSQLRepository()
        {
            _connectionString = @"Data Source=DMKIM\MSSQLSERVER1;Integrated Security=True;Initial Catalog=PBD_Opcenter;";
        }

        public bool Create(Equipment obj)
        {
            throw new NotImplementedException();
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Equipment>> GetAllAsync()
        {
            return await Task.Run(() =>
            {
                return GetAll();
            });

        }

        public IEnumerable<Equipment> GetAll()
        {
            IEnumerable<QueryEquipmentSql> list;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "p_ImportEquipment_FromPBD_ToMES";
                connection.Open();
                list = connection.Query<QueryEquipmentSql>(sql, new { BD = 1 }, commandType: CommandType.StoredProcedure);
            };
            var resultList = new List<Equipment>();
            progressStatus = 0;
            progressMaxStatus = list.Count();

            foreach (var el in list)
            {
                progressStatus++;

                var dmEqElName = el.Name;
                var dmEqElUID = el.UID;
                var dmEqElBD = el.BD;
                Equipment eqElement = new Equipment
                {
                    Name = dmEqElName,
                    Description = $"1C UID:{el.UID}",
                    NId = dmEqElName,
                    LevelNId = "ProductionUnit"
                };
                resultList.Add(eqElement);

            }
            return resultList;
        }

        public IEnumerable<Equipment> GetAllWithCreation()
        {
            IEnumerable<QueryEquipmentSql> list;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "p_ImportEquipment_FromPBD_ToMES";
                connection.Open();
                list = connection.Query<QueryEquipmentSql>(sql, new { BD = 1 }, commandType: CommandType.StoredProcedure);
            };
            var resultList = new List<Equipment>();
            foreach (var el in list)
            {
                var dmEqElName = el.Name;
                var dmEqElUID = el.UID;
                var dmEqElBD = el.BD;
                EquipmentConfiguration eqConf = new EquipmentConfiguration
                {
                    LevelNId = "ProductionUnit",
                    NId = dmEqElName,
                    Name = dmEqElName,
                    Description = $"1C UID:{el.UID}",
                    EquipmentTypeNId ="Unit",
                };
                /*
                if (EqConfRepo.GetById(dmEqElName)==null)
                {
                   EqConfRepo.Create(eqConf);
                }
                var eqConfNewEl = EqConfRepo.GetById(dmEqElName);

                Equipment eqElement = new Equipment
                {
                    Name = dmEqElName,
                    Description = $"1C UID:{el.UID}",
                    NId = dmEqElName,
                    LevelNId = "ProductionUnit",
                    EquipmentConfigurationId = eqConfNewEl.Id
                };
                if (EqRepo.GetById(dmEqElName)==null)
                {
                    //EqRepo.Create(eqElement);
                }
                resultList.Add(eqElement);
                */
            }
            return resultList;
        }

        public IEnumerable<Equipment> GetAllByDateInterval(DateTime dateFrom, TimeSpan dateOffset)
        {
            throw new NotImplementedException();
        }

        public Equipment GetById(string id)
        {
            throw new NotImplementedException();
        }

        public void Update(Equipment obj)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            //LeanFactory.ShutDown();
        }

        ~Equipment1CSQLRepository()
        {
            Dispose();

        }


    }
}

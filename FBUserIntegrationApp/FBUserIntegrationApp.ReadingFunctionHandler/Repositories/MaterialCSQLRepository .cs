﻿using _1СDataServiceSQL.DAL.DTModels;
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
    public class Material1CSQLRepository : IRepositorySQL1C<DM_Material>,IDisposable
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
            UoMRepo = new UoMRepository(unifiedSdkLean);
        }

        public void DisposeRepositories()
        {
            //unifiedSdkLean.Dispose();
        }

        public Material1CSQLRepository(IUnifiedSdkLean unifiedSdkLean = null)
        {
            this.unifiedSdkLean = unifiedSdkLean;
            _connectionString = @"Data Source=DMKIM\MSSQLSERVER1;Integrated Security=True;Initial Catalog=PBD_Preactor;";
            if (unifiedSdkLean == null)
            {
                initSdkLean();
            }
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
        
        public IEnumerable<DM_Material> GetAllWithCreation()
        {
            IEnumerable<QueryMaterialSql> list;
            using (var connection = new SqlConnection(_connectionString))
            {
                var sql = "p_ImportMaterial_FromPBD_ToMES";
                connection.Open();
                list = connection.Query<QueryMaterialSql>(sql, new { BD = 1 }, commandType: CommandType.StoredProcedure);
            };
            DM_MaterialRepository dm = new DM_MaterialRepository(unifiedSdkLean);
            var resultList = new List<DM_Material>();

            foreach (var el in list)
            {
                uom = UoMRepo.GetByName("Unit");
                var dmMatElKod = el.PartNo;
                var dmMatElName = el.Product;
                var dmMatEl = dm_MatRepo.GetByNId(dmMatElKod);
                if (dmMatEl == null)
                {
                    CreateDM_Material(dmMatElKod, dmMatElName);
                    dmMatEl = dm_MatRepo.GetByNId(dmMatElKod);
                }
                resultList.Add(dmMatEl);
            }
            return resultList;
        }
        public async Task<IEnumerable<DM_Material>> GetAllAsync()
        {
            return await Task.Run(()=>
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
            DM_MaterialRepository dm = new DM_MaterialRepository(unifiedSdkLean);
            var resultList = new List<DM_Material>();
            progressStatus = 0;
            progressMaxStatus = list.Count();

            foreach (var el in list)
            {
                progressStatus++;
                uom = UoMRepo.GetByName("Unit");
                var dmMatElKod = el.PartNo;
                var dmMatElName = el.Product;
                //var dmMatElItem = dm_MatRepo.GetByNId(dmMatElKod);

                var dmMatEl = dm_MatRepo.GetByNId(dmMatElKod);
                if (dmMatEl == null)
                {
                    var matNewInstance = new Material
                    {
                        Name = dmMatElName,
                        NId = dmMatElKod,
                        Revision = "A",
                        UoMNId = UoMRepo.GetByName("Unit").NId
                    };
                    var dmNewMat = new DM_Material
                    {
                        Material = matNewInstance,
                        LogisticClassNId = "Default",
                        MaterialClass_Id = dm_MatClassesRepo.GetByName("n/a").Id,
                        Material_Id = matNewInstance.Id
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

        public void Dispose()
        {
            //LeanFactory.ShutDown();
        }

        ~Material1CSQLRepository()
        {
            DisposeRepositories();
            Dispose();

        }


    }
}

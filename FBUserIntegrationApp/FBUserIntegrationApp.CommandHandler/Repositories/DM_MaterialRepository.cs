using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMCDataService.DAL.Interfaces;
using System.Net;
using System.IO;
using System.Xml;
using Siemens.SimaticIT.Unified.Common.Information;
using IntegrationApp.FBUserIntegrationApp.FBUserIntegrationApp.IAPOMModel.DataModel.ReadingModel;
using Siemens.SimaticIT.Unified;
using IntegrationApp.FBUserIntegrationApp.FBUserIntegrationApp.IAPOMModel.Commands.Published;
using Siemens.SimaticIT.UADM.MasterData.FB_MS_MAT.MSModel.Commands;
using CreateDM_Material = IntegrationApp.FBUserIntegrationApp.FBUserIntegrationApp.IAPOMModel.Commands.Published.CreateDM_Material;

namespace IntegrationLibrary.MasterData.FBIntegrationLibrary.MSModel.DAL
{
    public class DM_MaterialRepository : IRepository<DM_Material>
    {
        IQueryable<DM_Material> DM_MaterialDataSource;
        private IUnifiedSdkComposite platform;
        public DM_MaterialRepository(IUnifiedSdkComposite platform)
        {
            this.platform = platform;
            DM_MaterialDataSource = (IQueryable<DM_Material>)platform.ProjectionQuery(typeof(DM_Material));
        }
        public IEnumerable<DM_Material> GetAll()
        {
            IQueryable<DM_Material> queriedEntity = DM_MaterialDataSource;
            return queriedEntity.Where(x => x.IsDeleted == 0).Include(p => p.Material).Include(p => p.MaterialClass);
        }

        public IEnumerable<DM_Material> GetAllByDateInterval(DateTime dateFrom, TimeSpan dateOffset)
        {
            DateTime dateTo = dateFrom.Add(dateOffset);
            IQueryable<DM_Material> queriedEntity = DM_MaterialDataSource.Where(x => 
                x.CreatedOn >= dateFrom
                && x.CreatedOn <= dateTo
                && x.IsDeleted == 0
                ); 
            return queriedEntity.Include(p => p.Material).Include(p => p.MaterialClass);
        }

        public bool Create(DM_Material obj)
        {
            CreateDM_Material.Response response = this.platform.CallCommand<CreateDM_Material, CreateDM_Material.Response>(
                new CreateDM_Material
                {
                      CorrelationId = obj.CorrelationId,
                      LogisticClassNId = obj.LogisticClassNId,
                      MaterialId = obj.Material_Id.Value,
                      MaterialClassId = obj.MaterialClass_Id.Value
                });
            return response.Succeeded;
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public DM_Material GetById(string id)
        {
            IQueryable<DM_Material> queriedEntity = DM_MaterialDataSource;
            return queriedEntity.Where(x => x.IsDeleted == 0 && x.Material_Id == new Guid(id)).Include(p => p.Material).Include(p => p.MaterialClass).FirstOrDefault();
        }

        public DM_Material GetByNId(string MatNId)
        {
            IQueryable<DM_Material> queriedEntity = DM_MaterialDataSource;
            return queriedEntity.Where(x => x.IsDeleted == 0 && x.Material.NId==MatNId).Include(p => p.Material).Include(p => p.MaterialClass).FirstOrDefault();
        }


        public bool Update(DM_Material obj)
        {
            UpdateDM_Material.Response response = this.platform.CallCommand<UpdateDM_Material, UpdateDM_Material.Response>(
                new UpdateDM_Material
                {
                    LogisticClassNId = obj.LogisticClassNId,
                    Id = obj.Id
                    
                });
            return response.Succeeded;
        }

    }
}

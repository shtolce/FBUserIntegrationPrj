using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMCDataService.DAL.Interfaces;
using System.Net;
using System.IO;
using System.Xml;
using Siemens.SimaticIT.Unified;
using IntegrationApp.FBUserIntegrationApp.FBUserIntegrationApp.IAPOMModel.DataModel.ReadingModel;
using Siemens.SimaticIT.MasterData.MAT_MS.MSModel.Commands;

namespace IntegrationLibrary.MasterData.FBIntegrationLibrary.MSModel.DAL
{
    public class MaterialRepository : IRepository<Material>
    {
        IQueryable<Material> MaterialDataSource;
        private IUnifiedSdkComposite platform;
        public MaterialRepository(IUnifiedSdkComposite platform)
        {
            this.platform = platform;
            MaterialDataSource = (IQueryable<Material>)platform.ProjectionQuery(typeof(Material));
        }
        public IEnumerable<Material> GetAll()
        {
            IQueryable<Material> queriedEntity = MaterialDataSource;
            return queriedEntity.Where(x => x.IsDeleted == 0);
        }

        public IEnumerable<Material> GetAllByDateInterval(DateTime dateFrom, TimeSpan dateOffset)
        {
            DateTime dateTo = dateFrom.Add(dateOffset);
            IQueryable<Material> queriedEntity = MaterialDataSource.Where(x => 
                x.CreatedOn >= dateFrom
                && x.CreatedOn <= dateTo
                && x.IsDeleted == 0
                ); 
            return queriedEntity;
        }

        public bool Create(Material obj)
        {
            CreateMaterial.Response response = this.platform.CallCommand<CreateMaterial, CreateMaterial.Response>(
                new CreateMaterial
                {
                     NId = obj.NId,
                     Name = obj.Name,
                     Description = obj?.Description,
                     Revision = obj?.Revision,
                     TemplateNId = obj?.TemplateNId,
                     UId = obj?.UId,
                     UoMNId = obj?.UoMNId
                     
                });
            return response.Succeeded;
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Material GetByName(string name)
        {
            IQueryable<Material> queriedEntity = MaterialDataSource;
            return queriedEntity.Where(x => x.IsDeleted == 0 && x.Name == name).FirstOrDefault();
        }

        public Material GetById(string id)
        {
            IQueryable<Material> queriedEntity = MaterialDataSource;
            return queriedEntity.Where(x => x.IsDeleted == 0 && x.NId == id).FirstOrDefault();
        }

        public void Update(Material obj)
        {
            throw new NotImplementedException();
        }
    }
}

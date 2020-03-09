using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMCDataService.DAL.Interfaces;
using System.Net;
using System.IO;
using System.Xml;
using IntegrationApp.FBUserIntegrationApp.FBUserIntegrationApp.IAPOMModel.DataModel.ReadingModel;
using Siemens.SimaticIT.Unified;
using Siemens.SimaticIT.MasterData.MAT_MS.MSModel.Commands;
using Siemens.SimaticIT.Unified.Common;

namespace IntegrationLibrary.MasterData.FBIntegrationLibrary.MSModel.DAL
{
    public class MaterialGroupRepository : IRepository<MaterialGroup>
    {
        IQueryable<MaterialGroup> MaterialGroupDataSource;
        private IUnifiedSdkReadingFunction platform;
        public MaterialGroupRepository(IUnifiedSdkComposite platform)
        {
            this.platform = platform;
            MaterialGroupDataSource = (IQueryable<MaterialGroup>)platform.ProjectionQuery(typeof(MaterialGroup));
        }
        public IEnumerable<MaterialGroup> GetAll()
        {
            IQueryable<MaterialGroup> queriedEntity = MaterialGroupDataSource;
            return queriedEntity.Where(x => x.IsDeleted == 0);
        }

        public IEnumerable<MaterialGroup> GetAllByDateInterval(DateTime dateFrom, TimeSpan dateOffset)
        {
            DateTime dateTo = dateFrom.Add(dateOffset);
            IQueryable<MaterialGroup> queriedEntity = MaterialGroupDataSource.Where(x => 
                x.CreatedOn >= dateFrom
                && x.CreatedOn <= dateTo
                && x.IsDeleted == 0
                ); 
            return queriedEntity;
        }

        public bool Create(MaterialGroup obj)
        {
            CreateMaterialGroup.Response response = this.platform.CallCommand<CreateMaterialGroup, CreateMaterialGroup.Response>(
                new CreateMaterialGroup
                {
                     NId = obj.NId,
                     Name = obj.Name,
                     Description = obj.Description
                });
            return response.Succeeded;
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public MaterialGroup GetByName(string name)
        {
            IQueryable<MaterialGroup> queriedEntity = MaterialGroupDataSource;
            return queriedEntity.Where(x => x.IsDeleted == 0 && x.Name == name).FirstOrDefault();
        }

        public MaterialGroup GetById(string id)
        {
            IQueryable<MaterialGroup> queriedEntity = MaterialGroupDataSource;
            return queriedEntity.Where(x => x.IsDeleted == 0 && x.NId == id).FirstOrDefault();
        }

        public void Update(MaterialGroup obj)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMCDataService.DAL.Interfaces;
using System.Net;
using System.IO;
using System.Xml;
using Siemens.SimaticIT.ReferenceData.UDM_RF.RFModel.Types;
using Siemens.SimaticIT.Unified.Common.Information;
using IntegrationApp.FBUserIntegrationApp.FBUserIntegrationApp.IAPOMModel.DataModel.ReadingModel;
using Siemens.SimaticIT.Unified;
using Siemens.SimaticIT.MasterData.FB_MS_BOM.MSModel.Commands;

namespace UMCDataService.DAL.Repositories
{
    public class BoMRepository : IRepository<BoM>
    {
        IQueryable<BoM> BoMDataSource;
        private IUnifiedSdkComposite platform;
        public BoMRepository(IUnifiedSdkComposite platform)
        {
            this.platform = platform;
            BoMDataSource = (IQueryable<BoM>)platform.ProjectionQuery(typeof(BoM));
        }
        public IEnumerable<BoM> GetAll()
        {
            IQueryable<BoM> queriedEntity = BoMDataSource;
            return queriedEntity.Where(x => x.IsDeleted == 0).Include(p=>p.BoMItems);
        }

        public IEnumerable<BoM> GetAllByDateInterval(DateTime dateFrom, TimeSpan dateOffset)
        {
            DateTime dateTo = dateFrom.Add(dateOffset);
            IQueryable<BoM> queriedEntity = BoMDataSource.Where(x => 
                x.CreatedOn >= dateFrom
                && x.CreatedOn <= dateTo
                && x.IsDeleted == 0
                ).Include(p => p.BoMItems); 
            return queriedEntity;
        }

        public bool Create(BoM obj)
        {
            CreateBoM.Response response = this.platform.CallCommand<CreateBoM, CreateBoM.Response>(
                new CreateBoM
                {
                     NId = obj.NId,
                     Name = obj.Name,
                     Description = obj.Description,
                     MaterialDefinition = obj.MaterialDefinition_Id.Value,
                     Priority = obj.Priority,
                     Quantity = new QuantityParameterType {QuantityValue =  obj.Quantity.QuantityValue,UoMNId=obj.Quantity.UoMNId},
                     ValidFrom = obj.ValidityFrom,
                     ValidTo = obj.ValidityTo,
                     Version = obj.Version
                     
                     
                     
                });
            return response.Succeeded;
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public BoM GetByName(string name)
        {
            IQueryable<BoM> queriedEntity = BoMDataSource;
            return queriedEntity.Where(x => x.IsDeleted == 0 && x.Name == name).FirstOrDefault();
        }

        public BoM GetById(string id)
        {
            IQueryable<BoM> queriedEntity = BoMDataSource;
            return queriedEntity.Where(x => x.IsDeleted == 0 && x.NId == id).FirstOrDefault();
        }

        public bool Update(BoM obj)
        {
            throw new NotImplementedException();
        }

    }
}

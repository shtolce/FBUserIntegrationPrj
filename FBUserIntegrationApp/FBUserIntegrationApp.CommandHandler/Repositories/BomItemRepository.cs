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
using Siemens.SimaticIT.Unified;
using IntegrationApp.FBUserIntegrationApp.FBUserIntegrationApp.IAPOMModel.DataModel.ReadingModel;
using Siemens.SimaticIT.MasterData.FB_MS_BOM.MSModel.Commands;

namespace UMCDataService.DAL.Repositories
{
    public class BoMItemRepository : IRepository<BoMItem>
    {
        IQueryable<BoMItem> BoMItemDataSource;
        private IUnifiedSdkComposite platform;
        IQueryable<Equipment> EquipmentDataSource;
        public BoMItemRepository(IUnifiedSdkComposite platform)
        {
            this.platform = platform;
                BoMItemDataSource = (IQueryable<BoMItem>)platform.ProjectionQuery(typeof(BoMItem));
        }
        public IEnumerable<BoMItem> GetAll()
        {
            IQueryable<BoMItem> queriedEntity = BoMItemDataSource;
            return queriedEntity.Where(x => x.IsDeleted == 0).Include(p=>p.BoM);
        }

        public IEnumerable<BoMItem> GetAllByDateInterval(DateTime dateFrom, TimeSpan dateOffset)
        {
            DateTime dateTo = dateFrom.Add(dateOffset);
            IQueryable<BoMItem> queriedEntity = BoMItemDataSource.Where(x => 
                x.CreatedOn >= dateFrom
                && x.CreatedOn <= dateTo
                && x.IsDeleted == 0
                ).Include(p => p.BoM); 
            return queriedEntity;
        }

        public bool Create(BoMItem obj)
        {
            CreateBoMItem.Response response = this.platform.CallCommand<CreateBoMItem, CreateBoMItem.Response>(
                new CreateBoMItem
                {
                     NId = obj.NId,
                     BoM = obj.BoM.Id,
                     Quantity =new QuantityParameterType {QuantityValue = obj.Quantity.QuantityValue,UoMNId = obj.Quantity.UoMNId },
                     MaterialDefinition = obj.MaterialDefinition.Id
                });
            return response.Succeeded;
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public BoMItem GetByName(string name)
        {
            IQueryable<BoMItem> queriedEntity = BoMItemDataSource;
            return queriedEntity.Where(x => x.IsDeleted == 0 && x.NId == name).FirstOrDefault();
        }

        public BoMItem GetById(string id)
        {
            IQueryable<BoMItem> queriedEntity = BoMItemDataSource;
            return queriedEntity.Where(x => x.IsDeleted == 0 && x.NId == id).FirstOrDefault();
        }

        public bool Update(BoMItem obj)
        {
            DeleteBoMItemList.Response response = this.platform.CallCommand<DeleteBoMItemList, DeleteBoMItemList.Response>(
                new DeleteBoMItemList
                {
                    Ids = new List<Guid>()
                    {
                        obj.Id
                    }
                });
            bool res = false;
            if (response.Succeeded)
                res = Create(obj);
            return res;
        }

    }
}

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
using Siemens.SimaticIT.U4DM.ReferenceData.FB_RD_SERV.RFModel.Commands;

namespace IntegrationLibrary.MasterData.FBIntegrationLibrary.MSModel.DAL
{
    public class UoMRepository : IRepository<UoM>
    {
        IQueryable<UoM> UoMDataSource;
        private IUnifiedSdkComposite platform;
            public UoMRepository(IUnifiedSdkComposite platform)
        {
                this.platform = platform;
                UoMDataSource = (IQueryable<UoM>)platform.ProjectionQuery(typeof(UoM));
        }
        public IEnumerable<UoM> GetAll()
        {
            IQueryable<UoM> queriedEntity = UoMDataSource;
            return queriedEntity.Where(x => x.IsDeleted == 0);
        }

        public IEnumerable<UoM> GetAllByDateInterval(DateTime dateFrom, TimeSpan dateOffset)
        {
            DateTime dateTo = dateFrom.Add(dateOffset);
            IQueryable<UoM> queriedEntity = UoMDataSource.Where(x => 
                x.CreatedOn >= dateFrom
                && x.CreatedOn <= dateTo
                && x.IsDeleted == 0
                ); 
            return queriedEntity;
        }

        public bool Create(UoM obj)
        {
            TCMCreateUoMList.Response response = this.platform.CallCommand<TCMCreateUoMList, TCMCreateUoMList.Response>(
                new TCMCreateUoMList
                {
                    NIds = new List<string> { obj.Name }
                });
            return response.Succeeded;
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public UoM GetByName(string name)
        {
            IQueryable<UoM> queriedEntity = UoMDataSource;
            return queriedEntity.Where(x => x.IsDeleted == 0 && x.Name == name).FirstOrDefault();
        }

        public UoM GetById(string id)
        {
            IQueryable<UoM> queriedEntity = UoMDataSource;
            return queriedEntity.Where(x => x.IsDeleted == 0 && x.NId == id).FirstOrDefault();
        }

        public bool Update(UoM obj)
        {
            throw new NotImplementedException();
        }
    }
}

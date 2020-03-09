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
using Siemens.SimaticIT.OperationalData.EQU_OP.OPModel.Commands;
using IntegrationApp.FBUserIntegrationApp.FBUserIntegrationApp.IAPOMModel.DataModel.ReadingModel;

namespace IntegrationLibrary.MasterData.FBIntegrationLibrary.MSModel.DAL
{
    public class EquipmentRepository : IRepository<Equipment>
    {
        private IUnifiedSdkComposite platform;
        IQueryable<Equipment> EquipmentDataSource;
        public EquipmentRepository(IUnifiedSdkComposite platform)
        {
            this.platform = platform;
            EquipmentDataSource = (IQueryable<Equipment>)platform.ProjectionQuery(typeof(Equipment));
        }
        public IEnumerable<Equipment> GetAll()
        {
            IQueryable<Equipment> queriedEntity = EquipmentDataSource;
            return queriedEntity.Where(x => x.IsDeleted == 0);
        }

        public IEnumerable<Equipment> GetAllByDateInterval(DateTime dateFrom, TimeSpan dateOffset)
        {
            DateTime dateTo = dateFrom.Add(dateOffset);
            IQueryable<Equipment> queriedEntity = EquipmentDataSource.Where(x => 
                x.CreatedOn >= dateFrom
                && x.CreatedOn <= dateTo
                && x.IsDeleted == 0
                ); 
            return queriedEntity;
        }

        public bool Create(Equipment obj)
        {
            CreateEquipment.Response response = platform.CallCommand<CreateEquipment, CreateEquipment.Response>(
                new CreateEquipment
                {
                    EquipmentConfigurationId = obj.EquipmentConfigurationId.Value
                     
                });
            return response.Succeeded;
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public Equipment GetByName(string name)
        {
            IQueryable<Equipment> queriedEntity = EquipmentDataSource;
            return queriedEntity.Where(x => x.IsDeleted == 0 && x.Name == name).FirstOrDefault();
        }

        public Equipment GetById(string id)
        {
            IQueryable<Equipment> queriedEntity = EquipmentDataSource;
            return queriedEntity.Where(x => x.IsDeleted == 0 && x.NId == id).FirstOrDefault();
        }

        public bool Update(Equipment obj)
        {
            throw new NotImplementedException();
        }

    }
}

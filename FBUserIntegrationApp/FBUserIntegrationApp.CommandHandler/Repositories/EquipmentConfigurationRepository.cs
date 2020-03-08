using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UMCDataService.DAL.Interfaces;
using System.Net;
using System.IO;
using System.Xml;
using Siemens.SimaticIT.MasterData.EQU_MS.MSModel.Commands;
using Siemens.SimaticIT.Unified.Common;
using Siemens.SimaticIT.Unified.Common.Information;
using Siemens.SimaticIT.Unified;
using IntegrationApp.FBUserIntegrationApp.FBUserIntegrationApp.IAPOMModel.DataModel.ReadingModel;
using Siemens.SimaticIT.MasterData.EQU_MS.MSModel.Types;
using Siemens.SimaticIT.ReferenceData.UDM_RF.RFModel.Types;

namespace IntegrationLibrary.MasterData.FBIntegrationLibrary.MSModel.DAL
{
    public class EquipmentConfigurationRepository : IRepository<EquipmentConfiguration>
    {
        IUnifiedSdkComposite platform;
        IQueryable<EquipmentConfiguration> EquipmentConfigurationDataSource;
        public EquipmentConfigurationRepository(IUnifiedSdkComposite platform)
        {
            this.platform = platform;
            EquipmentConfigurationDataSource = platform.ProjectionQuery<EquipmentConfiguration>();
        }

        public IEnumerable<EquipmentConfiguration> GetAll()
        {
            IQueryable<EquipmentConfiguration> queriedEntity = EquipmentConfigurationDataSource;
            return queriedEntity.Where(x => x.IsDeleted == 0);
        }

        public IEnumerable<EquipmentConfiguration> GetAllByDateInterval(DateTime dateFrom, TimeSpan dateOffset)
        {
            DateTime dateTo = dateFrom.Add(dateOffset);
            IQueryable<EquipmentConfiguration> queriedEntity = EquipmentConfigurationDataSource.Where(x => 
                x.CreatedOn >= dateFrom
                && x.CreatedOn <= dateTo
                && x.IsDeleted == 0
                ); 
            return queriedEntity;
        }

        public bool Create(EquipmentConfiguration obj)
        {
            CreateEquipmentConfiguration.Response response = platform.CallCommand<CreateEquipmentConfiguration, CreateEquipmentConfiguration.Response>(
                new CreateEquipmentConfiguration
                {
                    Description = obj.Description,
                    EquipmentTypeNId = obj.EquipmentTypeNId,
                    LevelNId = obj.LevelNId,
                    LocationNId = obj.LocationNId,
                    Name = obj.Name,
                    NId = obj.NId,
                    Properties = new List<PropertyWithAttributesParameterType>()
                    {
                        new PropertyWithAttributesParameterType()
                        {
                            NId = "IsActive",
                            PropertyValue = "True",
                            PropertyType = SupportedTypes.Bool,
                            Attributes =new List<PropertyParameterType>()
                        },
                        new PropertyWithAttributesParameterType()
                        {
                            NId = "Locable",
                            PropertyValue = "False",
                            PropertyType = SupportedTypes.Bool,
                            Attributes =new List<PropertyParameterType>()
                        },
                        new PropertyWithAttributesParameterType()
                        {
                            NId = "Is3DPrinter",
                            PropertyValue = "False",
                            PropertyType = SupportedTypes.Bool,
                            Attributes =new List<PropertyParameterType>()
                        },
                        new PropertyWithAttributesParameterType()
                        {
                            NId = "ActiveNonConformanceNr",
                            PropertyValue = "0",
                            PropertyType = SupportedTypes.Int,
                            Attributes =new List<PropertyParameterType>()
                        },
                        new PropertyWithAttributesParameterType()
                        {
                            NId = "EnabledForCompleteByDifferentUser",
                            PropertyValue = "False",
                            PropertyType = SupportedTypes.Bool,
                            Attributes =new List<PropertyParameterType>()
                        }
                    }

                });

          



            return response.Succeeded;
        }

        public void Delete(int id)
        {
            throw new NotImplementedException();
        }

        public EquipmentConfiguration GetByName(string name)
        {
            IQueryable<EquipmentConfiguration> queriedEntity = EquipmentConfigurationDataSource;
            return queriedEntity.Where(x => x.IsDeleted == 0 && x.Name == name).FirstOrDefault();
        }

        public EquipmentConfiguration GetById(string id)
        {
            IQueryable<EquipmentConfiguration> queriedEntity = EquipmentConfigurationDataSource;
            return queriedEntity.Where(x => x.IsDeleted == 0 && x.NId == id).FirstOrDefault();
        }

        private List<PropertyParameterType> getNewProperties(EquipmentConfiguration obj)
        {

            List<PropertyParameterType> Properties = new List<PropertyParameterType>();


            if (obj.EquipmentConfigurationProperties.FirstOrDefault(el => el.NId == "IsActive")==null)
                Properties.Add(
                            new PropertyParameterType()
                            {
                                NId = "IsActive",
                                PropertyValue = "True",
                                PropertyType = SupportedTypes.Bool
                            });
            if (obj.EquipmentConfigurationProperties.FirstOrDefault(el => el.NId == "Lockable") == null)
                Properties.Add(
                        new PropertyParameterType()
                        {
                            NId = "Lockable",
                            PropertyValue = "False",
                            PropertyType = SupportedTypes.Bool
                        });
            if (obj.EquipmentConfigurationProperties.FirstOrDefault(el => el.NId == "Is3DPrinter") == null)
                Properties.Add(
                        new PropertyParameterType()
                        {
                            NId = "Is3DPrinter",
                            PropertyValue = "False",
                            PropertyType = SupportedTypes.Bool
                        });
            if (obj.EquipmentConfigurationProperties.FirstOrDefault(el => el.NId == "ActiveNonConformanceNr") == null)
                Properties.Add(
                        new PropertyParameterType()
                        {
                            NId = "ActiveNonConformanceNr",
                            PropertyValue = "0",
                            PropertyType = SupportedTypes.Int
                        });

            if (obj.EquipmentConfigurationProperties.FirstOrDefault(el => el.NId == "EnabledForCompleteByDifferentUser") == null)
                Properties.Add(
                        new PropertyParameterType()
                        {
                            NId = "EnabledForCompleteByDifferentUser",
                            PropertyValue = "False",
                            PropertyType = SupportedTypes.Bool
                        });
            return Properties;
        }


        public void Update(EquipmentConfiguration obj, EquipmentConfiguration foundEl)
        {
            List<PropertyParameterType> newProperties = getNewProperties(foundEl);
            UpdateEquipmentConfiguration.Response response = platform.CallCommand<UpdateEquipmentConfiguration, UpdateEquipmentConfiguration.Response>(
                new UpdateEquipmentConfiguration
                {
                    Id = obj.Id,
                    Description = obj.Description,
                    LevelNId = obj.LevelNId,
                    LocationNId = obj.LocationNId,
                    Name = obj.Name,
                    Properties = newProperties
                });
            foreach (EquipmentConfigurationProperty pr in foundEl.EquipmentConfigurationProperties)
            {
                UpdateEquipmentConfigurationProperty.Response resp = platform.CallCommand<UpdateEquipmentConfigurationProperty, UpdateEquipmentConfigurationProperty.Response>(
                    new UpdateEquipmentConfigurationProperty
                    {
                        Id = pr.Id,
                        PropertyValue = pr.PropertyValue// пока заглушка, ничего не обновляем
                    });


            }





        }

        public void Update(EquipmentConfiguration obj)
        {
            throw new NotImplementedException();
        }
    }
}

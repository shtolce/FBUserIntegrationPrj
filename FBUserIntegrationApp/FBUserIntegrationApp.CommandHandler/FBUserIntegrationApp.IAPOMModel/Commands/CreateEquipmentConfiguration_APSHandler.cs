using System;
using System.Collections.Generic;
using System.Linq;
using Siemens.SimaticIT.Unified.Common;
using Siemens.SimaticIT.Unified.Common.Information;
using Siemens.SimaticIT.Handler;
using Siemens.SimaticIT.Unified;
using Siemens.SimaticIT.SDK.Diagnostics.Tracing;
using IntegrationApp.FBUserIntegrationApp.FBUserIntegrationApp.IAPOMModel.DataModel.ReadingModel;
using IntegrationLibrary.MasterData.FBIntegrationLibrary.MSModel.DAL;

namespace IntegrationApp.FBUserIntegrationApp.FBUserIntegrationApp.IAPOMModel.Commands
{
    [Handler(HandlerCategory.BasicMethod)]
    public partial class CreateEquipmentConfiguration_APSHandlerShell 
    {
        IUnifiedSdkComposite _platform;
        private void CreateEquipment(string Name, string NId, string LevelNId, string Description, ITracer tracer)
        {
            var dmEqElName = Name;
            var dmEqElUID = Description;
            var dmEqElDescription = Description;
            EquipmentConfiguration eqConf = new EquipmentConfiguration
            {
                LevelNId = "ProductionUnit",
                NId = dmEqElName,
                Name = dmEqElName,
                Description = dmEqElDescription,
                EquipmentTypeNId = "Unit"
                
            };
            var EqConfRepo = new EquipmentConfigurationRepository(_platform);
            var EqRepo = new EquipmentRepository(_platform);

            EquipmentConfiguration foundEl = EqConfRepo.GetById(dmEqElName);
            if (foundEl == null)
            {
                EqConfRepo.Create(eqConf);
                tracer.Write("Siemens-SimaticIT-Trace-BusinessLogic",
                    Siemens.SimaticIT.SDK.Diagnostics.Common.Category.Informational, "Создан Resource" + dmEqElName);
            }
            else
            {
                eqConf.Id = foundEl.Id;
                EqConfRepo.Update(eqConf, foundEl);
                tracer.Write("Siemens-SimaticIT-Trace-BusinessLogic",
                    Siemens.SimaticIT.SDK.Diagnostics.Common.Category.Informational, "Изменен Resource" + dmEqElName);
            }
            var eqConfNewEl = EqConfRepo.GetById(dmEqElName);

            Equipment eqElement = new Equipment
            {
                Name = dmEqElName,
                Description = dmEqElDescription,
                NId = dmEqElName,
                LevelNId = "ProductionUnit",
                EquipmentConfigurationId = eqConfNewEl.Id
            };
            if (EqRepo.GetById(dmEqElName) == null)
            {
                //EqRepo.Create(eqElement);
            }
        }

        [HandlerEntryPoint]
        private CreateEquipmentConfiguration_APS.Response CreateEquipmentConfiguration_APSHandler(CreateEquipmentConfiguration_APS command, IUnifiedSdkComposite platform)
        {
            this._platform = platform;
            ITracer tracer = platform.Tracer;
            CreateEquipment(command.Name, command.NId, command.LevelNId, command.Description,tracer);
            return new CreateEquipmentConfiguration_APS.Response()
            {
                SitUafExecutionDetail = new SitUafExecutionDetail(new List<PrecheckExecution>() { new PrecheckExecution("successcful", "successcful", null) })
            };
                                
        }
    }
}










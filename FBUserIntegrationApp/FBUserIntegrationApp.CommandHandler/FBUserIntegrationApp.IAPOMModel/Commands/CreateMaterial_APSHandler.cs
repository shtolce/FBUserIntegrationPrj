using System;
using System.Collections.Generic;
using System.Linq;
using Siemens.SimaticIT.Unified.Common;
using Siemens.SimaticIT.Unified.Common.Information;
using IntegrationApp.FBUserIntegrationApp.FBUserIntegrationApp.IAPOMModel.Commands.Published;
using Siemens.SimaticIT.Handler;
using Siemens.SimaticIT.Unified;
using Siemens.SimaticIT.SDK.Diagnostics.Tracing;
using IntegrationLibrary.MasterData.FBIntegrationLibrary.MSModel.DAL;
using IntegrationApp.FBUserIntegrationApp.FBUserIntegrationApp.IAPOMModel.DataModel.ReadingModel;

namespace IntegrationApp.FBUserIntegrationApp.FBUserIntegrationApp.IAPOMModel.Commands
{
    [Handler(HandlerCategory.BasicMethod)]
    public partial class CreateMaterial_APSHandlerShell 
    {
        IUnifiedSdkComposite _platform;
        MaterialGroupRepository dm_MatClassesRepo;
        DM_MaterialRepository dm_MatRepo;
        MaterialRepository matRepo;
        UoMRepository UoMRepo;

        public bool CreateDM_Material(string dmMatElKod, string dmMatElName, string dmRevision, string dmLogisticClassNId)
        {
            var matNewInstance = new Material
            {
                Name = dmMatElName,
                NId = dmMatElKod,
                Revision = dmRevision,
                UoMNId = UoMRepo.GetByName("Unit").NId
            };
            matRepo.Create(matNewInstance);
            matNewInstance = matRepo.GetById(dmMatElKod);
            var dmNewMat = new DM_Material
            {
                Material = matNewInstance,
                LogisticClassNId = dmLogisticClassNId,
                MaterialClass_Id = dm_MatClassesRepo.GetByName("n/a").Id,
                Material_Id = matNewInstance.Id
            };
            
            return dm_MatRepo.Create(dmNewMat);
        }

        public bool UpdateDM_Material(DM_Material dmMatEl, string dmMatElKod, string dmMatElName,string dmRevision, string dmLogisticClassNId)
        {
            var matNewInstance = new Material
            {
                Name = dmMatElName,
                NId = dmMatElKod,
                Revision = dmRevision,
                UoMNId = UoMRepo.GetByName("Unit").NId
            };
            matRepo.Update(matNewInstance);
            matNewInstance = matRepo.GetById(dmMatElKod);
            var dmNewMat = new DM_Material
            {
                Material = matNewInstance,
                LogisticClassNId = dmLogisticClassNId,
                MaterialClass_Id = dm_MatClassesRepo.GetByName("n/a").Id,
                Material_Id = matNewInstance.Id
            };
            dmNewMat.Id = dmMatEl.Id;
            return dm_MatRepo.Update(dmNewMat);
        }

        private void CreateMaterial(CreateMaterial_APS command, ITracer tracer)
        {
            UoM uom = UoMRepo.GetByName("Unit");
            var dmMatElKod = command.NId;
            var dmMatElName = command.Name;
            var dmRevision = command.Revision;
            var dmLogisticClassNId = command.LogisticClassNId;
            DM_Material dmMatEl = dm_MatRepo.GetByNId(dmMatElKod);
            if (dmMatEl == null)
            {
                if (CreateDM_Material(dmMatElKod, dmMatElName, dmRevision, dmLogisticClassNId) == true)
                {
                    dmMatEl = dm_MatRepo.GetByNId(dmMatElKod);
                    tracer.Write("Siemens-SimaticIT-Trace-BusinessLogic",
                        Siemens.SimaticIT.SDK.Diagnostics.Common.Category.Informational, "Создан Material " + dmMatElName);
                }
                else
                {
                    tracer.Write("Siemens-SimaticIT-Trace-BusinessLogic",
                    Siemens.SimaticIT.SDK.Diagnostics.Common.Category.Informational, "Не удалось создать Material " + dmMatElName);
                }

            }
            else
            {
                if (UpdateDM_Material(dmMatEl,dmMatElKod, dmMatElName, dmRevision, dmLogisticClassNId) == true)
                {
                    dmMatEl = dm_MatRepo.GetByNId(dmMatElKod);
                    tracer.Write("Siemens-SimaticIT-Trace-BusinessLogic",
                    Siemens.SimaticIT.SDK.Diagnostics.Common.Category.Informational, "Изменен Material " + dmMatElName);
                }
                else
                {
                    tracer.Write("Siemens-SimaticIT-Trace-BusinessLogic",
                    Siemens.SimaticIT.SDK.Diagnostics.Common.Category.Informational, "Не удалось обновить Material " + dmMatElName);
                }

            }

        }


        [HandlerEntryPoint]
        private CreateMaterial_APS.Response CreateMaterial_APSHandler(CreateMaterial_APS command)
        {
            this._platform = platform;
            dm_MatClassesRepo = new MaterialGroupRepository(_platform);
            dm_MatRepo = new DM_MaterialRepository(_platform);
            matRepo = new MaterialRepository(_platform);
            UoMRepo = new UoMRepository(_platform);
            ITracer tracer = platform.Tracer;
            CreateMaterial(command, tracer);
            return new CreateMaterial_APS.Response()
            {
                SitUafExecutionDetail = new SitUafExecutionDetail(new List<PrecheckExecution>() { new PrecheckExecution("successcful", "successcful", null) })
            };

                                
        }
    }
}

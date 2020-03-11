using System;
using System.Collections.Generic;
using System.Linq;
using Siemens.SimaticIT.Unified.Common;
using Siemens.SimaticIT.Unified.Common.Information;
using IntegrationApp.FBUserIntegrationApp.FBUserIntegrationApp.IAPOMModel.Commands.Published;
using Siemens.SimaticIT.Handler;
using Siemens.SimaticIT.Unified;
using _1СDataServiceSQL.DAL.DTModels;
using IntegrationApp.FBUserIntegrationApp.FBUserIntegrationApp.IAPOMModel.DataModel.ReadingModel;
using IntegrationLibrary.MasterData.FBIntegrationLibrary.MSModel.DAL;
using Siemens.SimaticIT.SDK.Diagnostics.Tracing;
using Siemens.SimaticIT.ReferenceData.UDM_RF.RFModel.Types;
using UMCDataService.DAL.Repositories;
using QT = Siemens.SimaticIT.ReferenceData.UDM_RF.RFModel.Types.ReadingModel.QuantityType;



namespace IntegrationApp.FBUserIntegrationApp.FBUserIntegrationApp.IAPOMModel.Commands
{
    [Handler(HandlerCategory.BasicMethod)]
    public partial class CreateBoM_APSHandlerShell 
    {
        UoMRepository UoMRepo;
        IUnifiedSdkComposite _platform;
        DM_MaterialRepository dm_MatRepo;
        MaterialGroupRepository dm_MatClassesRepo;
        MaterialRepository matRepo;
        BoMItemRepository BoMItemRepo;
        BoMRepository BoMRepoMES;
        public bool CreateDM_Material(string dmMatElKod, string dmMatElName, string dmRevision="", string dmLogisticClassNId="")
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

        public bool UpdateDM_Material(DM_Material dmMatEl, string dmMatElKod, string dmMatElName, string dmRevision, string dmLogisticClassNId)
        {
            var matFound = matRepo.GetById(dmMatElKod);

            var matNewInstance = new Material
            {
                Id = matFound.Id,
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

        public void UpdateBoMItem(string dmMatElKodItem, string dmMatElNameItem, decimal? qty, UoM uom, BoM newBoMEl)
        {
            var dmMatElItem = dm_MatRepo.GetByNId(dmMatElKodItem);
            if (dmMatElItem == null)
            {
                CreateDM_Material(dmMatElKodItem, dmMatElNameItem);
                dmMatElItem = dm_MatRepo.GetByNId(dmMatElKodItem);
            }

            BoMItem bItemNew = new BoMItem
            {
                BoM_Id = newBoMEl?.Id,
                BoM = newBoMEl,
                NId = dmMatElKodItem,
                Quantity = new QT { QuantityValue = qty, UoMNId = uom?.NId },
                MaterialDefinition = dmMatElItem
            };
            BoMItemRepo.Update(bItemNew);

        }



        public void CreateBoMItem(string dmMatElKodItem, string dmMatElNameItem, decimal? qty, UoM uom, BoM newBoMEl)
        {
            var dmMatElItem = dm_MatRepo.GetByNId(dmMatElKodItem);
            if (dmMatElItem == null)
            {
                CreateDM_Material(dmMatElKodItem, dmMatElNameItem);
                dmMatElItem = dm_MatRepo.GetByNId(dmMatElKodItem);
            }

            BoMItem bItemNew = new BoMItem
            {
                BoM_Id = newBoMEl?.Id,
                BoM = newBoMEl,
                NId = dmMatElKodItem,
                Quantity = new QT { QuantityValue = qty, UoMNId = uom?.NId },
                MaterialDefinition = dmMatElItem
            };
            BoMItemRepo.Create(bItemNew);
        }

        public void CreateBoM(BoMDTO el,ITracer tracer)
        {

            BoM newBoMEl = null;
            UoM uom;
            uom = UoMRepo.GetByName("Unit");
            var dmMatElKod = el.NId;
            var dmMatElName = el.Name;
            var qtyBoM = new QT { QuantityValue = (decimal?)el.Quantity, UoMNId = uom.NId };

            var dmMatEl = dm_MatRepo.GetByNId(dmMatElKod);
            if (dmMatEl == null)
            {
                CreateDM_Material(dmMatElKod, dmMatElName);
                dmMatEl = dm_MatRepo.GetByNId(dmMatElKod);
            }
            var BoMEl = BoMRepoMES.GetById(dmMatElKod);
            if (BoMEl == null)
            {
                newBoMEl = new BoM
                {
                    Description = dmMatElName,
                    Name = dmMatElName,
                    NId = dmMatElKod,
                    Quantity = qtyBoM,
                    BoMItems = new List<BoMItem>(),
                    MaterialDefinition = dmMatEl,
                    MaterialDefinition_Id = dmMatEl.Id,
                    Version = el.Version
                };
                if (BoMRepoMES.Create(newBoMEl)==true)
                    tracer.Write("Siemens-SimaticIT-Trace-BusinessLogic",
                            Siemens.SimaticIT.SDK.Diagnostics.Common.Category.Informational, "Создан BoM " + dmMatElName);
                else
                    tracer.Write("Siemens-SimaticIT-Trace-BusinessLogic",
                            Siemens.SimaticIT.SDK.Diagnostics.Common.Category.Informational, "Не удалось создать BoM " + dmMatElName);

            }

        }


        [HandlerEntryPoint]
        private CreateBoM_APS.Response CreateBoM_APSHandler(CreateBoM_APS command, IUnifiedSdkComposite platform)
        {

            this._platform = platform;
            ITracer tracer = platform.Tracer;
            dm_MatClassesRepo = new MaterialGroupRepository(_platform);
            dm_MatRepo = new DM_MaterialRepository(_platform);
            matRepo = new MaterialRepository(_platform);
            UoMRepo = new UoMRepository(_platform);
            BoMItemRepo = new BoMItemRepository(_platform);
            BoMRepoMES = new BoMRepository(_platform);

            var BoMEl = BoMRepoMES.GetById(command.NId);
            if (BoMEl == null)
            {
                BoMDTO el = new BoMDTO
                {
                    Name = command.Name,
                    Version = command.Version,
                    NId = command.NId,
                    Quantity = command.Quantity
                };
                CreateBoM(el, tracer);
                BoMEl = BoMRepoMES.GetById(command.NId);

            }
            else
            {
                    tracer.Write("Siemens-SimaticIT-Trace-BusinessLogic",
                            Siemens.SimaticIT.SDK.Diagnostics.Common.Category.Informational, "Найден элемент BoM " + command.Name);

            }

            var uom = UoMRepo.GetByName("Unit");
            var dmMatElItemKod = command.BoMItem_NId;
            var dmMatElItemName = command.BoMItem_Name;
            decimal qtyBoMItemParse = command.BoMItem_Quantity.Value;
            var qtyBoMItem = new QuantityType { QuantityValue = (decimal?)qtyBoMItemParse, UoMNId = uom.NId };

            BoMItem bItem = BoMItemRepo.GetById(dmMatElItemKod);
            if (bItem ==null)
            {
                CreateBoMItem(dmMatElItemKod, dmMatElItemName, qtyBoMItem.QuantityValue, uom, BoMEl);
                tracer.Write("Siemens-SimaticIT-Trace-BusinessLogic",
                        Siemens.SimaticIT.SDK.Diagnostics.Common.Category.Informational, "Найден элемент BoMItem " + dmMatElItemName);

            }
            else
            {
                UpdateBoMItem(dmMatElItemKod, dmMatElItemName, qtyBoMItem.QuantityValue, uom, BoMEl);
                tracer.Write("Siemens-SimaticIT-Trace-BusinessLogic",
                        Siemens.SimaticIT.SDK.Diagnostics.Common.Category.Informational, "Обновлен элемент BoMItem " + dmMatElItemName);
            }

            return new CreateBoM_APS.Response()
            {
                SitUafExecutionDetail = new SitUafExecutionDetail(new List<PrecheckExecution>() { new PrecheckExecution("successcful", "successcful", null) })
            };

        }
    }
}

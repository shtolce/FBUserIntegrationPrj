
using System;
using System.Collections.Generic;
using System.Linq;
using Siemens.SimaticIT.Unified.Common;
using Siemens.SimaticIT.Unified.Common.Information;
using Siemens.SimaticIT.Handler;
using Siemens.SimaticIT.Unified;
using APSDataService.DAL;
using IntegrationApp.FBUserIntegrationApp.FBUserIntegrationApp.IAPOMModel.DataModel.ReadingModel;

namespace IntegrationApp.FBUserIntegrationApp.FBUserIntegrationApp.IAPOMModel.ReadingFunctions
{
    [Handler(HandlerCategory.BasicMethod)]
    public partial class GetMaterialAPSHandlerShell 
    {
        [HandlerEntryPoint]
        private FunctionResponse<GetMaterialAPS.FunctionResponse> GetMaterialAPSHandler(GetMaterialAPS readingFunction, IUnifiedSdkReadingFunction platform)
        {
            Material1CSQLRepository mRepo = new Material1CSQLRepository();
            IEnumerable<DM_Material> mList = mRepo?.GetAll();
            List<GetMaterialAPS.FunctionResponse> result = new List<GetMaterialAPS.FunctionResponse>();
            foreach (DM_Material el in mList)
                result?.Add(new GetMaterialAPS.FunctionResponse()
                {
                    Name = el.Material?.Name,
                    NId = el.Material?.NId,
                    LogisticClassNId = el.LogisticClassNId,
                    MaterialClass_Id = el.MaterialClass_Id.ToString(),
                    Material_Id = el.MaterialClass_Id.ToString(),
                    Revision = el.Material.Revision,
                    UoMNId = el.Material.UoMNId
                    
                    
                });


            return new FunctionResponse<GetMaterialAPS.FunctionResponse>()
            {
                Data = result?.AsQueryable(),
            };


        }
    }
}

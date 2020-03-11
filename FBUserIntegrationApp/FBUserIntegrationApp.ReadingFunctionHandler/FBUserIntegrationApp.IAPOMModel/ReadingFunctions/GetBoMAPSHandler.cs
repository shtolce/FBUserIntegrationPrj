
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
    public partial class GetBoMAPSHandlerShell 
    {
        [HandlerEntryPoint]
        private FunctionResponse<GetBoMAPS.FunctionResponse> GetBoMAPSHandler(GetBoMAPS readingFunction)
        {

            BoM1CSQLRepository eqRepo = new BoM1CSQLRepository();
            IEnumerable<BoM> eqList = eqRepo?.GetAll();
            List<GetBoMAPS.FunctionResponse> result = new List<GetBoMAPS.FunctionResponse>();
            foreach (BoM el in eqList)
                foreach (BoMItem item in el.BoMItems)
                    result?.Add(new GetBoMAPS.FunctionResponse()
                    {
                        Name = el.Name,
                        NId = el.NId,
                        Description = el.Description,
                        Quantity  =el.Quantity.QuantityValue,
                        Version = el.Version,
                        MaterialDefinition_Name = el.MaterialDefinition?.Material?.Name,
                        MaterialDefinition_NId = el.MaterialDefinition?.Material?.NId,
                        BoMItem_MaterialDefinition_Name = item.MaterialDefinition?.Material?.Name,
                        BoMItem_MaterialDefinition_NId = item.MaterialDefinition?.Material?.NId,
                        BoMItem_Name = item.MaterialDefinition?.Material?.Name,
                        BoMItem_NId = item.MaterialDefinition?.Material?.NId,
                        BoMItem_Quantity = item.Quantity.QuantityValue
                    });

            return new FunctionResponse<GetBoMAPS.FunctionResponse>()
            {
                Data = result?.AsQueryable(),
            };

                     
        }
    }
}


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
    /// <summary>
    /// Partial class init
    /// </summary>
    [Handler(HandlerCategory.BasicMethod)]
    public partial class GetEquipmentAPSHandlerShell 
    {
        /// <summary>
        /// This is the handler the MES engineer should write
        /// This is the ENTRY POINT for the user in VS IDE
        /// </summary>
        /// <param name="readingFunction"></param>
        /// <returns></returns>
        [HandlerEntryPoint]
        private FunctionResponse<GetEquipmentAPS.FunctionResponse> GetEquipmentAPSHandler(GetEquipmentAPS readingFunction)
        {
            Equipment1CSQLRepository eqRepo = new Equipment1CSQLRepository();
            IEnumerable<Equipment> eqList = eqRepo?.GetAll();
            List<GetEquipmentAPS.FunctionResponse> result = new List<GetEquipmentAPS.FunctionResponse>();
            foreach (Equipment el in eqList)
                result?.Add(new GetEquipmentAPS.FunctionResponse()
                {
                    Name = el?.Name,
                    NId = el?.NId,
                    LevelNId = el?.LevelNId,
                    Description = el?.Description,
                });

            return new FunctionResponse<GetEquipmentAPS.FunctionResponse>()
            {
                Data = result?.AsQueryable(),
            };

        }
    }
}

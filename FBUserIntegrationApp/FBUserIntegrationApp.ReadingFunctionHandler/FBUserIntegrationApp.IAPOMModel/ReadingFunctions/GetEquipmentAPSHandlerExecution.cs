using System;

using Siemens.SimaticIT.Unified.Common;
using Siemens.SimaticIT.Unified;

namespace IntegrationApp.FBUserIntegrationApp.FBUserIntegrationApp.IAPOMModel.ReadingFunctions
{
    /// <summary>
    /// Class initialize
    /// </summary>
    public partial class GetEquipmentAPSHandlerShell : IReadingFunctionHandler
    {
        private IUnifiedSdkReadingFunction  Platform;
        
        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="unifiedSdk"></param>
        /// <param name="readingFunction"></param>
        /// <returns></returns>
        public FunctionResponse Execute(IUnifiedSdkReadingFunction unifiedSdk, IReadingFunction readingFunction)
        {
            this.Platform = unifiedSdk;

            return GetEquipmentAPSHandler((GetEquipmentAPS)readingFunction);
        }

        /// <summary>
        /// Retrieve the type of the readingFunction
        /// </summary>
        public global::System.Type GetReadingFunctionType()
        {
            return typeof(GetEquipmentAPS);
        }
    }
}

using System;

using Siemens.SimaticIT.Unified.Common;
using Siemens.SimaticIT.Unified;

namespace IntegrationApp.FBUserIntegrationApp.FBUserIntegrationApp.IAPOMModel.Commands
{
    /// <summary>
    /// Class initialize
    /// </summary>
    public partial class CreateEquipmentConfiguration_APSHandlerShell : ICompositeCommandHandler
    {
        private IUnifiedSdkComposite platform;
        
        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="unifiedSdkComposite"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        public Response Execute(IUnifiedSdkComposite unifiedSdkComposite, ICommand command)
        {
            platform = unifiedSdkComposite;

            return CreateEquipmentConfiguration_APSHandler((CreateEquipmentConfiguration_APS)command,platform);
        }

        /// <summary>
        /// Retrieve the type of the command
        /// </summary>
        public global::System.Type GetCommandType()
        {
            return typeof(CreateEquipmentConfiguration_APS);
        }
    }
}

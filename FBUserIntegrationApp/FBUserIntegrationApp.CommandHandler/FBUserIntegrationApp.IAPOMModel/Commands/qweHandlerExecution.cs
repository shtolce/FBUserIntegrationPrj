using System;

using Siemens.SimaticIT.Unified.Common;
using Siemens.SimaticIT.Unified;

namespace IntegrationApp.FBUserIntegrationApp.FBUserIntegrationApp.IAPOMModel.Commands
{
    /// <summary>
    /// Class initialize
    /// </summary>
    public partial class qweHandlerShell : ICompositeCommandHandler
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

            return qweHandler((qwe)command);
        }

        /// <summary>
        /// Retrieve the type of the command
        /// </summary>
        public global::System.Type GetCommandType()
        {
            return typeof(qwe);
        }
    }
}

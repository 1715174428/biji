using log4net;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;

namespace IM.Web.Filters
{
    public class SignalRHubPipelineModule: HubPipelineModule
    {
        private static ILog log = LogManager.GetLogger(typeof(SignalRHubPipelineModule));

        protected override void OnIncomingError(ExceptionContext exceptionContext, IHubIncomingInvokerContext invokerContext)
        {
            base.OnIncomingError(exceptionContext, invokerContext);
            log.Error("SignalR发生未处理异常", exceptionContext.Error);
        }
    }
}
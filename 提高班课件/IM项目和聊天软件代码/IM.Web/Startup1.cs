using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.AspNet.SignalR;
using IM.Web.Filters;
using Newtonsoft.Json;

[assembly: OwinStartup(typeof(IM.Web.Startup1))]

namespace IM.Web
{
    public class Startup1
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
            GlobalHost.HubPipeline.AddModule(new SignalRHubPipelineModule());

            //SignalR的json序列化改成“驼峰命名法”
            var settings = new JsonSerializerSettings();
            settings.ContractResolver = new SignalRContractResolver();
            GlobalHost.DependencyResolver.Register(typeof(JsonSerializer), () => JsonSerializer.Create(settings));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;

namespace UserCenter.OpenAPI
{
    public class VersionControllerSelector : DefaultHttpControllerSelector
    {
        private HttpConfiguration config;
        private IDictionary<string, HttpControllerDescriptor> ctlMapping;
        public VersionControllerSelector(HttpConfiguration config):base(config)
        {
            this.config = config;
        }

        public override IDictionary<string, HttpControllerDescriptor> GetControllerMapping()
        {
            Dictionary<string, HttpControllerDescriptor> dict = new Dictionary<string, HttpControllerDescriptor>();
            foreach(var asm in config.Services.GetAssembliesResolver().GetAssemblies())
            {
                var controllerTypes = asm.GetTypes()
                    .Where(t=>t.IsAbstract==false&&typeof(ApiController).IsAssignableFrom(t));
                foreach(var ctrlType in controllerTypes)
                {
                    string ctrlTypeNS = ctrlType.Namespace;
                    var match = Regex.Match(ctrlTypeNS, @"\.v(\d)");
                    if (!match.Success)
                    {
                        continue;
                    }
                    string verNum = match.Groups[1].Value;//把版本号1提取出来
                    string ctrlTypeName = ctrlType.Name;//PersonController
                    var matchControler = Regex.Match(ctrlTypeName, "^(.+)Controller$");
                    if (!matchControler.Success)
                    {
                        continue;
                    }
                    string ctrlName = matchControler.Groups[1].Value;//得到Person
                    string key = ctrlName +"v"+ verNum;//Personv1
                    dict[key] = new HttpControllerDescriptor(config,ctrlName,ctrlType);
                }
            }
            ctlMapping = dict;
            return dict;
        }

        public override HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            string controller = (string)request.GetRouteData().Values["controller"];
            //controller =>Person
            if(ctlMapping==null)
            {
                ctlMapping = GetControllerMapping();
            }
            ///api/v2/Person
            var matchVer = Regex.Match(request.RequestUri.PathAndQuery, @"/v(\d+)/");
            if(!matchVer.Success)
            {
                return base.SelectController(request);
            }
            string verNum = matchVer.Groups[1].Value;//2
            string key = controller + "v" + verNum;
            if(ctlMapping.ContainsKey(key))
            {
                return ctlMapping[key];
            }
            else
            {
                return base.SelectController(request);
            }            
        }
    }
}
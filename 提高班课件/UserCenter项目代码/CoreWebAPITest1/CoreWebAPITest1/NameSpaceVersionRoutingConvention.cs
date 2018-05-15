using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System.Linq;
using System.Text.RegularExpressions;

namespace CoreWebAPITest1
{
    public class NameSpaceVersionRoutingConvention : IApplicationModelConvention
    {
        private string prefix;

        public NameSpaceVersionRoutingConvention(string prefix)
        {
            this.prefix = prefix;
        }

        public void Apply(ApplicationModel application)
        {
            var controllers = application.Controllers;
            foreach(var controller in controllers)
            {
                //判断Controller上是否标注了[Route]
                var hasRouteAttribute = 
                    controller.Selectors.Any(s=>s.AttributeRouteModel!=null);
                //如果标注了，则不由我处理
                if(hasRouteAttribute)
                {
                    continue;
                }
                var matchVer = Regex.Match(controller.ControllerType.Namespace,
                    @".v(\d+)");
                if(!matchVer.Success)
                {
                    continue;
                }
                string verNum = matchVer.Groups[1].Value;//1
                //计算这个Controller对应的路由路径
                string template = this.prefix + "/v" + verNum + "/" + controller.ControllerName;
                controller.Selectors[0].AttributeRouteModel = new AttributeRouteModel() { Template=template};
            }
        }
    }
}

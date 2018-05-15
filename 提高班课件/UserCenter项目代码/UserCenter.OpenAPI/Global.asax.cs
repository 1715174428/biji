using Autofac;
using Autofac.Integration.WebApi;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using UserCenter.IServices;
using UserCenter.OpenAPI.Filters;

namespace UserCenter.OpenAPI
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            InitAutoFac();
            GlobalConfiguration.Configure(WebApiConfig.Register);            
        }

        private void InitAutoFac()
        {
            var configuration = GlobalConfiguration.Configuration;
            var builder = new ContainerBuilder();

            // Register API controllers using assembly scanning. 
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly()).PropertiesAutowired();
            builder.RegisterWebApiFilterProvider(configuration);

            //一个对象必须是IOC容器创建出来的，IOC容器才会自动帮我们注入
            builder.RegisterType(typeof(UCAuthorizationFilter)).PropertiesAutowired();

            var services = Assembly.Load("UserCenter.Services");
            builder.RegisterAssemblyTypes(services)
                .Where(type => !type.IsAbstract && typeof(IServiceTag).IsAssignableFrom(type))
                .AsImplementedInterfaces().SingleInstance().PropertiesAutowired();

            var container = builder.Build();
            // Set the WebApi dependency resolver.  
            var resolver = new AutofacWebApiDependencyResolver(container);
            configuration.DependencyResolver = resolver;
        }

    }
}

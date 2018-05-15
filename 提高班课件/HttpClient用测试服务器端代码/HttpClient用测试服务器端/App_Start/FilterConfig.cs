using System.Web;
using System.Web.Mvc;

namespace HttpClient用测试服务器端
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}

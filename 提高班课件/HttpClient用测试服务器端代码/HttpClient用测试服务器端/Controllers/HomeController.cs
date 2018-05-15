using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HttpClient用测试服务器端.Controllers
{
    public class HomeController : Controller
    {
        [HttpPost]
        public ActionResult Login(string userName,string password)
        {
            if(userName == "admin"&&password=="123")
            {
                return Json("ok");
            }
            else
            {
                return Json("error");
            }
        }

        [HttpPost]
        public ActionResult Login2(Login2Request data)
        {
            //dynamic data = JsonConvert.DeserializeObject<dynamic>(content);
            string userName = data.userName;
            string password = data.password;
            if (userName == "admin" && password == "123")
            {
                return Json(data);
            }
            else
            {
                return Json(data);
            }
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            string userName = Request.Headers["UserName"];
            string password = Request.Headers["Password"];
            if (userName == "admin" && password == "123")
            {
                file.SaveAs(Server.MapPath("~/" + file.FileName));
                return Json("ok");
            }
            else
            {
                return Json("error");
            }
        }
    }
    public class Login2Request
    {
        public string userName { get; set; }
        public string password { get; set; }
    }
}
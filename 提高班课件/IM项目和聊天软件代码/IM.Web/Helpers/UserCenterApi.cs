using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using UserCenter.NETSDK;

namespace IM.Web
{
    public class UserCenterApi
    {
        private static string appKey;
        private static string appSecret;
        private static string appServerRoot;
        static UserCenterApi()
        {
            appKey = ConfigurationManager.AppSettings["UC_AppKey"];
            appSecret = ConfigurationManager.AppSettings["UC_AppSecret"];
            appServerRoot = ConfigurationManager.AppSettings["UC_ServerRoot"];
            User = new UserApi(appKey, appSecret, appServerRoot);
            UserGroup = new UserGroupApi(appKey, appSecret, appServerRoot);
        }

        public static readonly UserApi User;
        public static readonly UserGroupApi UserGroup;
    }
}
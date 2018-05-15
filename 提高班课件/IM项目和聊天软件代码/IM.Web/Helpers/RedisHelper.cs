using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace IM.Web
{
    public class RedisHelper
    {
        public static string Prefix = "RuPengIM_Web_";
        public static string Prefix_UserIsOnline = Prefix+ "UserIsOnline";
        public static string Prefix_GroupMessages = Prefix + "GroupMessages";

        private static string[] endPoints;
        static RedisHelper()
        {
            endPoints=ConfigurationManager.AppSettings["RedisServers"].Split(',', ':', '|');
        }

        public static ConnectionMultiplexer Create()
        {
            ConfigurationOptions opt = new ConfigurationOptions();
            foreach(string endPoint in endPoints)
            {
                opt.EndPoints.Add(endPoint);
            }
            return ConnectionMultiplexer.Connect(opt);
        }

    }
}
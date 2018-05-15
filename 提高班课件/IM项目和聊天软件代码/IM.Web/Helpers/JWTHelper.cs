using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using UserCenter.NETSDK;

namespace IM.Web
{
    public class JWTHelper
    {
        private static readonly string jwt_secret;

        static JWTHelper()
        {
            jwt_secret = ConfigurationManager.AppSettings["JWT_Secret"];
        }

        /// <summary>
        /// 把user加密放到JWT字符串中
        /// </summary>
        /// <param name="user"></param>
        /// <returns>JWT字符串</returns>
        public static string Encrypt(User user)
        {
            IJwtAlgorithm algorithm = new HMACSHA256Algorithm();
            IJsonSerializer serializer = new JsonNetSerializer();
            IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(algorithm, serializer, urlEncoder);

            var token = encoder.Encode(user, jwt_secret);
            return token;
        }

        /// <summary>
        /// 从JWT token中解密出来User
        /// </summary>
        /// <param name="token"></param>
        /// <returns>如果token错误、被篡改或者过期，则返回null</returns>
        public static User Decrypt(string token)
        {
            try
            {
                IJsonSerializer serializer = new JsonNetSerializer();
                IDateTimeProvider provider = new UtcDateTimeProvider();
                IJwtValidator validator = new JwtValidator(serializer, provider);
                IBase64UrlEncoder urlEncoder = new JwtBase64UrlEncoder();
                IJwtDecoder decoder = new JwtDecoder(serializer, validator, urlEncoder);

                var user = decoder.DecodeToObject<User>(token, jwt_secret, verify: true);
                return user;
            }
            catch (TokenExpiredException)
            {
                return null;
            }
            catch (SignatureVerificationException)
            {
                return null;
            }
        }

        /// <summary>
        /// 获得当前登录用户的User（供asp.net mvc用）
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        public static User GetUser(HttpContextBase httpContext)
        {
            var cookie = httpContext.Request.Cookies["JWTToken"];
            if(cookie==null)
            {
                return null;
            }
            string token = cookie.Value;
            return Decrypt(token);
        }

        /// <summary>
        /// 获得当前登录用户的User（供SignalR的Hub用）
        /// </summary>
        /// <param name="hubContext"></param>
        /// <returns></returns>
        public static User GetUser(HubCallerContext hubContext)
        {
            if(!hubContext.RequestCookies.ContainsKey("JWTToken"))
            {
                return null;
            }
            string token = hubContext.RequestCookies["JWTToken"].Value;
            return Decrypt(token);
        }
    }
}
using RuPeng.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace UserCenter.NETSDK
{
    class SDKClient
    {
        private string appKey;
        private string appSecret;
        private string serverRoot;//http://127.0.0.1:8888/api/v1/
        public SDKClient(string appKey, string appSecret, string serverRoot)
        {
            this.appKey = appKey;
            this.appSecret = appSecret;
            this.serverRoot = serverRoot;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url">要请求的地址</param>
        /// <param name="queryStringData">querystring参数键值对</param>
        /// <returns></returns>
        public async Task<SDKResult> GetAsync(string url,IDictionary<string,object> queryStringData)
        {
            // var orderedQS =  queryStringData.OrderBy(kv => kv.Key);
            //var qsItems = orderedQS.Select(kv=>kv.Key+"="+kv.Value);
            if(queryStringData==null)
            {
                throw new ArgumentNullException("queryStringData不能为null");
            }
            var qsItems = queryStringData.OrderBy(kv => kv.Key)
                .Select(kv => kv.Key + "=" + kv.Value);
            var queryString = string.Join("&", qsItems);
            string sign = MD5Helper.ComputeMd5(queryString + appSecret);

            using (HttpClient hc = new HttpClient())
            {
                hc.DefaultRequestHeaders.Add("AppKey", appKey);
                hc.DefaultRequestHeaders.Add("Sign", sign);
                var resp = await hc.GetAsync(serverRoot+url + "?" + queryString);
                SDKResult sdkResult = new SDKResult();
                sdkResult.Result = await resp.Content.ReadAsStringAsync();
                sdkResult.StatusCode = resp.StatusCode;
                return sdkResult;
            }
        }
    }
}

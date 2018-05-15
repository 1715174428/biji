using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserCenter.NETSDK
{
    public class UserApi
    {
        private string appKey;
        private string appSecret;
        private string serverRoot;
        public UserApi(string appKey, string appSecret, string serverRoot)
        {
            this.appKey = appKey;
            this.appSecret = appSecret;
            this.serverRoot = serverRoot;
        }

        public async Task<long> AddNewAsync(string phoneNum, string nickName, string password)
        {
            SDKClient client = new SDKClient(appKey,appSecret,serverRoot);
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["phoneNum"] = phoneNum;
            data["nickName"] = nickName;
            data["password"] = password;
            var result = await client.GetAsync("User/AddNew", data);
            if(result.StatusCode== System.Net.HttpStatusCode.OK)
            {
                //因为返回的报文体是新增id：{5}
                //使用newtonsoft把json格式反序列化为long
                long id = JsonConvert.DeserializeObject<long>(result.Result);
                return id;
            }
            else
            {
                throw new ApplicationException("新增失败，状态码"
                    +result.StatusCode+"，响应报文"+result.Result);
            }
        }

        public async Task<bool> UserExistsAsync(string phoneNum)
        {
            SDKClient client = new SDKClient(appKey, appSecret, serverRoot);
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["phoneNum"] = phoneNum;
            var result = await client.GetAsync("User/UserExists", data);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<bool>(result.Result);
            }
            else
            {
                throw new ApplicationException("UserExists失败，状态码"
                    + result.StatusCode + "，响应报文" + result.Result);
            }
        }

        public async Task<bool> CheckLoginAsync(string phoneNum, string password)
        {
            SDKClient client = new SDKClient(appKey, appSecret, serverRoot);
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["phoneNum"] = phoneNum;
            data["password"] = password;
            var result = await client.GetAsync("User/CheckLogin", data);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return JsonConvert.DeserializeObject<bool>(result.Result);
            }
            else
            {
                throw new ApplicationException("CheckLogin失败，状态码"
                    + result.StatusCode + "，响应报文" + result.Result);
            }
        }

        public async Task<User> GetByIdAsync(long id)
        {
            SDKClient client = new SDKClient(appKey, appSecret, serverRoot);
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["id"] = id;
            var result = await client.GetAsync("User/GetById", data);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                //因为返回的报文体是新增id：{5}
                //使用newtonsoft把json格式反序列化为long
                return  JsonConvert.DeserializeObject<User>(result.Result);
            }
            else
            {
                throw new ApplicationException("GetById失败，状态码"
                    + result.StatusCode + "，响应报文" + result.Result);
            }
        }

        public async Task<User> GetByPhoneNumAsync(string phoneNum)
        {
            SDKClient client = new SDKClient(appKey, appSecret, serverRoot);
            Dictionary<string, object> data = new Dictionary<string, object>();
            data["phoneNum"] = phoneNum;
            var result = await client.GetAsync("User/GetByPhoneNum", data);
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                //因为返回的报文体是新增id：{5}
                //使用newtonsoft把json格式反序列化为long
                return JsonConvert.DeserializeObject<User>(result.Result);
            }
            else
            {
                throw new ApplicationException("GetByPhoneNum失败，状态码"
                    + result.StatusCode + "，响应报文" + result.Result);
            }
        }
    }
}

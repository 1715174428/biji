using IM.Web.Models;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using UserCenter.NETSDK;

namespace IM.Web.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Login(string phoneNum,string password)
        {
            bool isSuccess = await UserCenterApi.User.CheckLoginAsync(phoneNum, password);
            var user = await UserCenterApi.User.GetByPhoneNumAsync(phoneNum);
            string token = JWTHelper.Encrypt(user);
            HttpCookie httpCookie = new HttpCookie("JWTToken", token) { Path="/",Expires=DateTime.Now.AddDays(30),HttpOnly=true};
            Response.SetCookie(httpCookie);
            string status = isSuccess ? "ok" : "error";
            return Json(new ReturnData { Status= status });
        }

        [HttpGet]
        public async Task<ActionResult> Main()
        {
            User user = JWTHelper.GetUser(HttpContext);
            return View(user);
        }

        [HttpGet]
        public async Task<ActionResult> GroupMain(long id)
        {
            User currentUser = JWTHelper.GetUser(HttpContext);
            var userGroups = await UserCenterApi.UserGroup.GetGroupsAsync(currentUser.Id);
            if (!userGroups.Select(g => g.Id).Contains(id))
            {
                ReturnData returnData = new ReturnData() { Status = "error", Msg = "当前用户不属于组" + id };
                return Json(returnData);
            }
            var userGroup = await UserCenterApi.UserGroup.GetByIdAsync(id);
            return View(userGroup);
        }

        [HttpPost]
        public async Task<ActionResult> LoadGroups()
        {
            User user = JWTHelper.GetUser(HttpContext);
            var groups = await UserCenterApi.UserGroup.GetGroupsAsync(user.Id);
            return Json(new ReturnData { Status="ok",Data=groups});
        }

        /// <summary>
        /// 加载组成员
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult> LoadGroupUsers(long groupId)
        {
            User currentUser = JWTHelper.GetUser(HttpContext);
            var userGroups = await UserCenterApi.UserGroup.GetGroupsAsync(currentUser.Id);
            if (!userGroups.Select(g => g.Id).Contains(groupId))
            {
                ReturnData returnData = new ReturnData() { Status = "error", Msg = "当前用户不属于组" + groupId };
                return Json(returnData);
            }
            //获取群组成员
            var groupUsers = await UserCenterApi.UserGroup.GetGroupUsersAsync(groupId);

            //创建返回值要用的GroupUserInfo集合
            //不返回手机号给客户端，避免泄密
            var users = groupUsers.Select(u=>new GroupUserInfo {Id=u.Id,NickName=u.NickName }).ToArray();

            RedisValue[] values;
            using (var redis = RedisHelper.Create())
            {
                var db = redis.GetDatabase();
                //redis中批量获取群组成员的在线状态
                RedisKey[] keys = users.Select(u =>(RedisKey)(RedisHelper.Prefix_UserIsOnline + u.Id)).ToArray();
                values = db.StringGet(keys);
            }
            //更新users中的在线状态
            for(int i= 0;i<users.Length;i++)
            {
                users[i].IsOnline = (bool)values[i];
            }

            return Json(new ReturnData() {Status="ok",Data= users });
        }

        /// <summary>
        /// 加载某一组的聊天记录
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public async Task<ActionResult> LoadHistoryMessages(long groupId)
        {
            User user = JWTHelper.GetUser(HttpContext);
            var userGroups = await UserCenterApi.UserGroup.GetGroupsAsync(user.Id);
            if (!userGroups.Select(g => g.Id).Contains(groupId))
            {
                ReturnData returnData = new ReturnData() { Status = "error", Msg = "当前用户不属于组" + groupId };
                return Json(returnData);
            }

            RedisValue[] jsons;
            using (var redis = RedisHelper.Create())
            {
                var db = redis.GetDatabase();
                //获取最近50条聊天记录，聊天记录是leftpush的
                jsons = await db.ListRangeAsync(RedisHelper.Prefix_GroupMessages + groupId,0,50);
            }
            //因为leftpush，取出来顺序是反的，需要颠倒过来，因此从后向前遍历
            List<GroupMessage> groupMessages = new List<GroupMessage>();
            for (int i = jsons.Length - 1; i >= 0; i--)
            {
                GroupMessage groupMsg = JsonConvert.DeserializeObject<GroupMessage>(jsons[i]);
                groupMessages.Add(groupMsg);
            }
            return Json(new ReturnData { Status="ok",Data=groupMessages});
        }
    }
}
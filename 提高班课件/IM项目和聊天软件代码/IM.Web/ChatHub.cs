using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using UserCenter.NETSDK;
using System.Threading.Tasks;
using StackExchange.Redis;
using Newtonsoft.Json;

namespace IM.Web
{
    public class ChatHub : Hub
    {
        public async Task<ReturnData> SendGroupMessage(long groupId,string message)
        {
            //获得当前登录用户信息
            User user = JWTHelper.GetUser(this.Context);
            //判断要发送的组是否属于用户所属组
            var userGroups = await  UserCenterApi.UserGroup.GetGroupsAsync(user.Id);
            if(!userGroups.Select(g=>g.Id).Contains(groupId))
            {
                ReturnData returnData = new ReturnData() { Status="error",Msg="当前用户不属于组"+groupId};
                return returnData;
            }
            {
                GroupMessage groupMsg = new GroupMessage();
                groupMsg.CreateDateTime = DateTime.Now;
                groupMsg.FromUserId = user.Id;
                groupMsg.FromUserNickName = user.NickName;
                groupMsg.Message = message;
                groupMsg.TargetGroupId = groupId;
                //SignalR通知消息
                this.Clients.Group(groupId.ToString()).onMessage(groupMsg);
            }
           
            //redis保存聊天记录，方便查看离线消息
            using (var redis = RedisHelper.Create())
            {
                GroupMessage groupMsg = new GroupMessage();
                groupMsg.CreateDateTime = DateTime.Now;
                groupMsg.FromUserId = user.Id;
                groupMsg.FromUserNickName = user.NickName;
                groupMsg.Message = message;
                groupMsg.TargetGroupId = groupId;
                string jsonGroupMsg = JsonConvert.SerializeObject(groupMsg);

                var db = redis.GetDatabase();
                await db.ListLeftPushAsync(RedisHelper.Prefix_GroupMessages + groupId, jsonGroupMsg);
                return new ReturnData { Status = "ok",Data=groupMsg };
            }            
        }

        public async override Task OnConnected()
        {
            User user = JWTHelper.GetUser(this.Context);

            //在redis中记录在线状态，方便后进来的人能获取在线状态/Home/LoadGroupUsers
            using (var redis = RedisHelper.Create())
            {
                IDatabase db = redis.GetDatabase();
                db.StringSet(RedisHelper.Prefix_UserIsOnline + user.Id, true);
            }

            //把用户加入SignalR组，以组的Id为groupName
            var groups = await UserCenterApi.UserGroup.GetGroupsAsync(user.Id);
            foreach(var group in groups)
            {
                string groupName = group.Id.ToString();
                await Groups.Add(this.Context.ConnectionId, groupName);
            }
            await base.OnConnected();
        }

        public async override Task OnDisconnected(bool stopCalled)
        {
            User user = JWTHelper.GetUser(this.Context);
            //更新在线状态
            using (var redis = RedisHelper.Create())
            {
                IDatabase db = redis.GetDatabase();
                db.StringSet(RedisHelper.Prefix_UserIsOnline + user.Id, false);
            }
            //把用户从SignalR组移除，提升效率，后续再次打开的时候还会从Redis加载离线消息
            //因此不会因为离开组而丢失消息
            var groups = await UserCenterApi.UserGroup.GetGroupsAsync(user.Id);
            foreach (var group in groups)
            {
                string groupName = group.Id.ToString();
                await Groups.Remove(this.Context.ConnectionId, groupName);
            }
            await base.OnDisconnected(stopCalled);
        }
    }
}
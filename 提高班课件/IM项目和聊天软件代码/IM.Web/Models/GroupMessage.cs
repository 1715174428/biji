using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IM.Web
{
    public class GroupMessage
    {
        public DateTime CreateDateTime { get; set; }
        public long FromUserId { get; set; }
        public string FromUserNickName { get; set; }
        public string Message { get; set; }
        public long TargetGroupId { get; set; }
    }
}
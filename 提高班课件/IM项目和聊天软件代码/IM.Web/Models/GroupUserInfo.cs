using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IM.Web.Models
{
    public class GroupUserInfo
    {
        public long Id { get; set; }
        public string NickName { get; set; }
        public bool IsOnline { get; set; }
    }
}
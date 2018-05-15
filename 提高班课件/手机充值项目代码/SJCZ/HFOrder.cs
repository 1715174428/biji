using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SJCZ
{
    public class HFOrder
    {
        public long Id { get; set; }
        public string PhoneNum { get; set; }
        public bool IsPayed { get; set; }
        public int Amount { get; set; }
        public byte[] Version { get; set; }
    }
}
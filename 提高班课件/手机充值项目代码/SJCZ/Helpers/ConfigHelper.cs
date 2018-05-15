using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SJCZ.Helpers
{
    public class ConfigHelper
    {
        public readonly static string AliPay_Partner = ConfigurationManager.AppSettings["AliPay.Partner"];
        public readonly static string AliPay_SecKey = ConfigurationManager.AppSettings["AliPay.SecKey"];
    }
}
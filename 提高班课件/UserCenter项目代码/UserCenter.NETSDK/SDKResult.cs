using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace UserCenter.NETSDK
{
    class SDKResult
    {
        /// <summary>
        /// 响应报文体
        /// </summary>
        public string Result { get; set; }

        public HttpStatusCode StatusCode { get; set; }
    }
}

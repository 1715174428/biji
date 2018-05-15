using SJCZ.Helpers;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SJCZ.Controllers
{
    public class DefaultController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(string phoneNum,int amount)
        {
            HFOrder order = new HFOrder();
            order.Amount = amount;
            order.IsPayed = false;
            order.PhoneNum = phoneNum;
            using (MyDbContext ctx = new MyDbContext())
            {                
                ctx.HFOrders.Add(order);
                ctx.SaveChanges();
            }

            var partner = ConfigHelper.AliPay_Partner;
            var secKey = ConfigHelper.AliPay_SecKey;
            //获取http://www.rupeng.com:8888这样的网站根路径
            string serverRoot = Request.Url.Scheme + "://" + Request.Url.Host + ":" + Request.Url.Port;
            var return_url = serverRoot+"/Default/PayReturn";
            var subject = "给" + phoneNum + "充值";
            var body = "给" + phoneNum + "充值";
            var out_trade_no = order.Id;
            var total_fee = amount;
            var seller_email = "yzk@rupeng.com";
            //计算sign
            string sign = Md5Helper.GetMD5(amount+partner+order.Id+subject+ secKey);
            string destUrl = "http://paytest.rupeng.cn/AliPay/PayGate.ashx?partner="+partner+ "&return_url="+Uri.EscapeDataString(return_url)+"&subject=" + Uri.EscapeDataString(subject)
                + "&body=" + Uri.EscapeDataString(body)
                + "&out_trade_no=" + out_trade_no
                + "&total_fee=" + total_fee
                + "&seller_email=" + Uri.EscapeDataString(seller_email)
                + "&sign=" + sign;
            return Redirect(destUrl);//让客户端重定向
        }

        //用户的重定向回调通知
        public ActionResult PayReturn(string out_trade_no, string returncode,int total_fee,string sign)
        {
            
            var secKey = ConfigHelper.AliPay_SecKey;
            //检查数据是否有被篡改
            string computedSign = Md5Helper.GetMD5(out_trade_no+returncode+total_fee+ secKey);
            //比对sign
            if(!computedSign.Equals(sign, StringComparison.OrdinalIgnoreCase))
            {
                return Content("sign校验失败，可能数据被篡改，请联系客服");
            }
            if (returncode != "ok")
             {
                return Content("支付失败");
            }
            long orderId = Convert.ToInt64(out_trade_no);
            using (MyDbContext ctx = new MyDbContext())
            {
                var order = ctx.HFOrders.SingleOrDefault(o => o.Id == orderId);
                if(order==null)
                {
                    return Content("订单找不到");
                }
                if(order.IsPayed)//避免重复充值
                {
                    return Content("充值已经成功，手机号"+order.PhoneNum+"，金额"+order.Amount);
                }
                if(order.Amount!= total_fee)
                {
                    return Content("订单金额和支付金额不符");
                }
                order.IsPayed = true;//更新为“订单已经支付”
                try
                {
                    ctx.SaveChanges();//如果SaveChanges不报错，说明数据没被别人修改，就可以后面的真正充值了
                    ChongZhi(order.PhoneNum, total_fee);
                    return Content("充值成功，手机号" + order.PhoneNum + "，金额" + order.Amount);
                }
                catch(DbUpdateConcurrencyException ex)//被并发修改了
                {
                    return Content("充值已经成功，手机号" + order.PhoneNum + "，金额" + order.Amount);
                }                
            }
        }

        //充值逻辑
        private void ChongZhi(string phoneNum,int amount)
        {

        }

        //被支付宝服务器直接的回调通知
        public ActionResult PayCallBack(string out_trade_no, string returncode, int total_fee, string sign)
        {
            var secKey = ConfigHelper.AliPay_SecKey;
            string computedSign = Md5Helper.GetMD5(out_trade_no + returncode + total_fee + secKey);
            if (!computedSign.Equals(sign, StringComparison.OrdinalIgnoreCase))
            {
                return Content("sign校验失败，可能数据被篡改");
            }
            long orderId = Convert.ToInt64(out_trade_no);
            using (MyDbContext ctx = new MyDbContext())
            {
                var order = ctx.HFOrders.SingleOrDefault(o => o.Id == orderId);
                if (order.IsPayed)//避免重复充值
                {
                    return Content("ok");
                }
                if (order.Amount != total_fee)
                {
                    return Content("订单金额和支付金额不符");
                }
                order.IsPayed = true;
                try
                {
                    ctx.SaveChanges();//如果SaveChanges不报错，说明数据没被别人修改，就可以后面的真正充值了
                    ChongZhi(order.PhoneNum, total_fee);
                    return Content("ok");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    return Content("ok");
                }
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Senparc.Weixin.TenPay.V2;
using Senparc.Weixin.TenPay.V3;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Open
{
    public partial class TenPayController
    {
        //[HttpGet, Route(Route)]
        //public async Task<ActionResult<RedirectResult>> Authorize([FromRoute] int siteId, [FromQuery] GetRequest request)
        //{
        //    try
        //    {
        //        var account = await _openAccountRepository.GetBySiteIdAsync(siteId);

        //        //var openId = User.Identity.Name;
        //        var openId = HttpContext.Session.GetString("OpenId");

        //        string sp_billno = Request.Query["order_no"];
        //        if (string.IsNullOrEmpty(sp_billno))
        //        {
        //            //生成订单10位序列号，此处用时间和随机数生成，商户根据自己调整，保证唯一
        //            sp_billno = string.Format("{0}{1}{2}", account.TenPayMchId/*10位*/, SystemTime.Now.ToString("yyyyMMddHHmmss"),
        //                StringUtils.GetRandomString(6));

        //            //注意：以上订单号仅作为演示使用，如果访问量比较大，建议增加订单流水号的去重检查。
        //        }
        //        else
        //        {
        //            sp_billno = Request.Query["order_no"];
        //        }

        //        var timeStamp = TenPayV3Util.GetTimestamp();
        //        var nonceStr = TenPayV3Util.GetNoncestr();

        //        var body = "test";
        //        var price = 100;//单位：分
        //        var xmlDataInfo = new TenPayV3UnifiedorderRequestData(TenPayV3Info.AppId, TenPayV3Info.MchId, body, sp_billno, price, HttpContext.UserHostAddress()?.ToString(), TenPayV3Info.TenPayV3Notify, TenPay.TenPayV3Type.JSAPI, openId, TenPayV3Info.Key, nonceStr);

        //        var result = TenPayV3.Unifiedorder(xmlDataInfo);//调用统一订单接口
        //                                                        //JsSdkUiPackage jsPackage = new JsSdkUiPackage(TenPayV3Info.AppId, timeStamp, nonceStr,);
        //        var package = string.Format("prepay_id={0}", result.prepay_id);

        //        ViewData["product"] = product;

        //        ViewData["appId"] = account.TenPayAppId;
        //        ViewData["timeStamp"] = timeStamp;
        //        ViewData["nonceStr"] = nonceStr;
        //        ViewData["package"] = package;
        //        ViewData["paySign"] = TenPayV3.GetJsPaySign(account.TenPayAppId, timeStamp, nonceStr, package, account.TenPayKey);

        //        //临时记录订单信息，留给退款申请接口测试使用
        //        HttpContext.Session.SetString("BillNo", sp_billno);
        //        HttpContext.Session.SetString("BillFee", price.ToString());

        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        var msg = ex.Message;
        //        msg += "<br>" + ex.StackTrace;
        //        msg += "<br>==Source==<br>" + ex.Source;

        //        if (ex.InnerException != null)
        //        {
        //            msg += "<br>===InnerException===<br>" + ex.InnerException.Message;
        //        }
        //        return Content(msg);
        //    }
        //}
    }
}

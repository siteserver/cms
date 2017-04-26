using SiteServer.CMS.WeiXin.WeiXinMP.Entities.JsonResult;

namespace SiteServer.CMS.WeiXin.WeiXinMP.AdvancedAPIs.QrCode
{
    /// <summary>
    /// 二维码创建返回结果
    /// </summary>
    public class CreateQrCodeResult : WxJsonResult
    {
        public string ticket { get; set; }
        public int expire_seconds { get; set; }
    }
}

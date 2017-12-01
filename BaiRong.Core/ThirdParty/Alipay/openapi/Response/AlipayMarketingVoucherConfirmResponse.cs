using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayMarketingVoucherConfirmResponse.
    /// </summary>
    public class AlipayMarketingVoucherConfirmResponse : AopResponse
    {
        /// <summary>
        /// 用于决定在用户确认领券后是否重定向。可枚举：true表示需要重定向，false表示不需要重定向
        /// </summary>
        [XmlElement("need_redirect")]
        public bool NeedRedirect { get; set; }

        /// <summary>
        /// 外部业务单号
        /// </summary>
        [XmlElement("out_biz_no")]
        public string OutBizNo { get; set; }

        /// <summary>
        /// 重定向地址，用于接收支付宝返回的领取码，必须使用utf-8编码
        /// </summary>
        [XmlElement("redirect_uri")]
        public string RedirectUri { get; set; }

        /// <summary>
        /// 发券码。如果发生异常sendCode为空
        /// </summary>
        [XmlElement("send_code")]
        public string SendCode { get; set; }

        /// <summary>
        /// 模板ID
        /// </summary>
        [XmlElement("template_id")]
        public string TemplateId { get; set; }

        /// <summary>
        /// 指定的用户确认页面主题名称
        /// </summary>
        [XmlElement("theme")]
        public string Theme { get; set; }

        /// <summary>
        /// 支付宝用户ID
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}

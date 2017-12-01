using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayMarketingVoucherAuthSendModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayMarketingVoucherAuthSendModel : AopObject
    {
        /// <summary>
        /// 外部业务订单号，用于幂等控制
        /// </summary>
        [XmlElement("out_biz_no")]
        public string OutBizNo { get; set; }

        /// <summary>
        /// 经过用户授权过后的发券码
        /// </summary>
        [XmlElement("send_code")]
        public string SendCode { get; set; }

        /// <summary>
        /// 券模板ID
        /// </summary>
        [XmlElement("template_id")]
        public string TemplateId { get; set; }

        /// <summary>
        /// 支付宝用户ID
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}

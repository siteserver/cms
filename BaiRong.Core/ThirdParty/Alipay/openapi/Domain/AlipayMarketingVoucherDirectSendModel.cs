using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayMarketingVoucherDirectSendModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayMarketingVoucherDirectSendModel : AopObject
    {
        /// <summary>
        /// 券金额(单位:分)
        /// </summary>
        [XmlElement("amount")]
        public string Amount { get; set; }

        /// <summary>
        /// 业务发生时间，格式为yyyy-MM-dd HH:mm:ss。
        /// </summary>
        [XmlElement("biz_date")]
        public string BizDate { get; set; }

        /// <summary>
        /// {"LEHUA_IS_ALGO_MONEY":"F","LEHUA_IS_MULTIPLIED":"F","LEHUA_MULTIPLIED_PRICE":"0.08","LEHUA_MULTIPLY_TIMES":"1.0","LEHUA_ORIGIN_PRICE":"0.08","camp_id":"1332546","camp_type":"PLATFORM_CAMP"}
        /// </summary>
        [XmlElement("extend_info")]
        public string ExtendInfo { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        [XmlElement("memo")]
        public string Memo { get; set; }

        /// <summary>
        /// 业务订单号，每次操作不可重复。
        /// </summary>
        [XmlElement("out_biz_no")]
        public string OutBizNo { get; set; }

        /// <summary>
        /// 模板编码，创建模板后生成。
        /// </summary>
        [XmlElement("template_id")]
        public string TemplateId { get; set; }

        /// <summary>
        /// USERID(支付宝用户2088账号)
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}

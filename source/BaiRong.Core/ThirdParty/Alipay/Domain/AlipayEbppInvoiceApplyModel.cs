using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEbppInvoiceApplyModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEbppInvoiceApplyModel : AopObject
    {
        /// <summary>
        /// 执行动作：申请开票/申请红冲  可选取值：  BLUE:申请开票;RED:申请红冲
        /// </summary>
        [XmlElement("action")]
        public string Action { get; set; }

        /// <summary>
        /// 申请发起方，描述开票申请的发起角色，  可选取值：  PAYEE:销售方;PAYER:购买方
        /// </summary>
        [XmlElement("apply_from")]
        public string ApplyFrom { get; set; }

        /// <summary>
        /// 发票申请内容
        /// </summary>
        [XmlElement("invoice_apply_model")]
        public InvoiceApplyOpenModel InvoiceApplyModel { get; set; }

        /// <summary>
        /// 定义商户的一级简称,用于标识商户品牌，对应于商户入驻时填写的"商户品牌简称"。  如：肯德基：KFC
        /// </summary>
        [XmlElement("m_short_name")]
        public string MShortName { get; set; }

        /// <summary>
        /// 定义商户的二级简称,用于标识商户品牌下的分支机构，如门店，对应于商户入驻时填写的"商户门店简称"。  如：肯德基-杭州西湖区文一西路店：KFC-HZ-19003  要求："商户品牌简称+商户门店简称"作为确定商户及其下属机构的唯一标识，不可重复。
        /// </summary>
        [XmlElement("sub_m_short_name")]
        public string SubMShortName { get; set; }

        /// <summary>
        /// 支付宝用户id，支付宝用户的唯一标识。
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}

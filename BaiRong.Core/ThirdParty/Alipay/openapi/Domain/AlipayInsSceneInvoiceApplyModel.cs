using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayInsSceneInvoiceApplyModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayInsSceneInvoiceApplyModel : AopObject
    {
        /// <summary>
        /// 发票寄送类型;ELECTRONIC:电子发票;PAPER:纸质发票；BOTH:电子+纸质
        /// </summary>
        [XmlElement("delivery_type")]
        public string DeliveryType { get; set; }

        /// <summary>
        /// 发票收件人
        /// </summary>
        [XmlElement("invoice_addressee")]
        public InsAddressee InvoiceAddressee { get; set; }

        /// <summary>
        /// 发票申请明细
        /// </summary>
        [XmlElement("invoice_apply_item")]
        public InsInvoiceApplyItem InvoiceApplyItem { get; set; }

        /// <summary>
        /// 申请发票开票日期
        /// </summary>
        [XmlElement("invoice_date")]
        public string InvoiceDate { get; set; }

        /// <summary>
        /// 发票抬头;收取发票的公司名称或个人姓名
        /// </summary>
        [XmlElement("invoice_title")]
        public string InvoiceTitle { get; set; }

        /// <summary>
        /// 开票类型;1:增值税普通发票（公司） ;2:增值税普通发票（个人） ;3:增值税专用发票.
        /// </summary>
        [XmlElement("invoice_type")]
        public string InvoiceType { get; set; }

        /// <summary>
        /// 商户业务单号
        /// </summary>
        [XmlElement("out_biz_no")]
        public string OutBizNo { get; set; }

        /// <summary>
        /// 商户生成的发票申请请求单号【幂等字段】
        /// </summary>
        [XmlElement("out_request_no")]
        public string OutRequestNo { get; set; }
    }
}

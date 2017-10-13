using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// InvoiceKeyInfo Data Structure.
    /// </summary>
    [Serializable]
    public class InvoiceKeyInfo : AopObject
    {
        /// <summary>
        /// 开票商户名称：商户品牌简称|商户门店简称
        /// </summary>
        [XmlElement("invoice_merchant_name")]
        public string InvoiceMerchantName { get; set; }

        /// <summary>
        /// 该交易是否支持开票
        /// </summary>
        [XmlElement("is_support_invoice")]
        public bool IsSupportInvoice { get; set; }

        /// <summary>
        /// 税号
        /// </summary>
        [XmlElement("tax_num")]
        public string TaxNum { get; set; }
    }
}

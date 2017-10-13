using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayDataDataserviceBillDownloadurlQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayDataDataserviceBillDownloadurlQueryModel : AopObject
    {
        /// <summary>
        /// 账单时间：日账单格式为yyyy-MM-dd，月账单格式为yyyy-MM。
        /// </summary>
        [XmlElement("bill_date")]
        public string BillDate { get; set; }

        /// <summary>
        /// 账单类型，商户通过接口或商户经开放平台授权后其所属服务商通过接口可以获取以下账单类型：trade、signcustomer；trade指商户基于支付宝交易收单的业务账单；signcustomer是指基于商户支付宝余额收入及支出等资金变动的帐务账单；
        /// </summary>
        [XmlElement("bill_type")]
        public string BillType { get; set; }
    }
}

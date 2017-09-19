using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayOfflineMarketProductBatchqueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayOfflineMarketProductBatchqueryModel : AopObject
    {
        /// <summary>
        /// 操作人角色，默认商户操作:MERCHANT；服务商操作：PROVIDER。支付宝内部使用，外部商户不需填写此字段。
        /// </summary>
        [XmlElement("op_role")]
        public string OpRole { get; set; }

        /// <summary>
        /// 页码，留空标示第一页，默认100个结果为一页
        /// </summary>
        [XmlElement("page_no")]
        public string PageNo { get; set; }

        /// <summary>
        /// 门店ID。支付宝内部使用，外部商户不需填写此字段。
        /// </summary>
        [XmlElement("shop_id")]
        public string ShopId { get; set; }
    }
}

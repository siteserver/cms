using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayCommerceTransportOfflinepayKeyQueryResponse.
    /// </summary>
    public class AlipayCommerceTransportOfflinepayKeyQueryResponse : AopResponse
    {
        /// <summary>
        /// 支付宝脱机交易公钥列表。列表中每一项为一个有效的支付宝公钥信息, 其中id字段表示支付宝公钥id。
        /// </summary>
        [XmlArray("keys")]
        [XmlArrayItem("alipay_offline_pay_master_key")]
        public List<AlipayOfflinePayMasterKey> Keys { get; set; }
    }
}

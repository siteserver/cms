using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayTradeAppMergePayModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayTradeAppMergePayModel : AopObject
    {
        /// <summary>
        /// 如果预创建成功，支付宝返回该预下单号，后续商户使用该预下单号请求支付宝支付接口
        /// </summary>
        [XmlElement("pre_order_no")]
        public string PreOrderNo { get; set; }
    }
}

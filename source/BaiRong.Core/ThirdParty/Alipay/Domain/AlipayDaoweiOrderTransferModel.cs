using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayDaoweiOrderTransferModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayDaoweiOrderTransferModel : AopObject
    {
        /// <summary>
        /// 备注信息。商户在推进订单状态时填写的备注信息，不超过500字符。
        /// </summary>
        [XmlElement("memo")]
        public string Memo { get; set; }

        /// <summary>
        /// 需要推进的订单状态。目前支持的订单动作是：START_SERVICE(派单模式服务开始)；PROVIDER_CONFIRMED (服务者完成服务)。
        /// </summary>
        [XmlElement("order_action")]
        public string OrderAction { get; set; }

        /// <summary>
        /// 到位业务订单号。用户在到位下单时，由到位系统生成的32位全局唯一数字 id。  通过应用中的应用网关post发送给商户（应用网关配置参考链接：https%3A%2F%2Fdoc.open.alipay.com%2Fdocs%2Fdoc.htm%3Fspm%3Da219a.7629140.0.0.TcIuKL%26treeId%3D193%26articleId%3D105310%26docType%3D1）。
        /// </summary>
        [XmlElement("order_no")]
        public string OrderNo { get; set; }
    }
}

using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayDaoweiOrderRefundModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayDaoweiOrderRefundModel : AopObject
    {
        /// <summary>
        /// 退款操作备注信息，用于详述退款单原因（使用该接口，必须要详细说明退款的原因），必填，长度不超过2000字符
        /// </summary>
        [XmlElement("memo")]
        public string Memo { get; set; }

        /// <summary>
        /// 到位业务订单号，全局唯一，由32位数字组成，用户在到位下单时系统生成并消息同步给商家，商户只能查自己同步到的订单号
        /// </summary>
        [XmlElement("order_no")]
        public string OrderNo { get; set; }

        /// <summary>
        /// 外部商户的退款id，用于控制退款操作的幂等，不同退款请求保证不同，最大长度不超过64字符
        /// </summary>
        [XmlElement("out_refund_id")]
        public string OutRefundId { get; set; }

        /// <summary>
        /// 退款金额，单位是元，商户可以全额退款也可以部分，退款金额不大于订单实际支付金额
        /// </summary>
        [XmlElement("refund_amount")]
        public string RefundAmount { get; set; }

        /// <summary>
        /// 订单退款的详细信息：可能包含多个服务订单的退款，内部包含每一个服务的订单号和单个的退款金额
        /// </summary>
        [XmlArray("refund_details")]
        [XmlArrayItem("order_refund_info")]
        public List<OrderRefundInfo> RefundDetails { get; set; }
    }
}

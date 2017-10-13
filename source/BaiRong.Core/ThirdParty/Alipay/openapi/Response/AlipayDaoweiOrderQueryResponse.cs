using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayDaoweiOrderQueryResponse.
    /// </summary>
    public class AlipayDaoweiOrderQueryResponse : AopResponse
    {
        /// <summary>
        /// 到位业务定义的订单买家id，全局唯一，商户可以根据该ID唯一确定买家的信息
        /// </summary>
        [XmlElement("buyer_user_id")]
        public string BuyerUserId { get; set; }

        /// <summary>
        /// 订单创建时间，用户点击预约下单操作的时间，格式为yyyy-MM-dd HH:mm:ss（到秒）下单时间因早于服务预约时间
        /// </summary>
        [XmlElement("gmt_create")]
        public string GmtCreate { get; set; }

        /// <summary>
        /// 订单修改时间，格式为yyyy-MM-dd HH:mm:ss(到秒，创建订单时，修改时间与创建时间相同)
        /// </summary>
        [XmlElement("gmt_modified")]
        public string GmtModified { get; set; }

        /// <summary>
        /// 订单最后支付时间，格式：yyyy-MM-dd HH:mm:ss（到秒）
        /// </summary>
        [XmlElement("gmt_payment")]
        public string GmtPayment { get; set; }

        /// <summary>
        /// 订单最后退款时间，格式：yyyy-MM-dd HH:mm:ss，订单产生退款时的最后操作时间
        /// </summary>
        [XmlElement("gmt_refund")]
        public string GmtRefund { get; set; }

        /// <summary>
        /// 物流信息，用户下订单填写的物流信息，包括服务地址的经纬度、联系人和手机号码以及扩展信息
        /// </summary>
        [XmlElement("logistics_info")]
        public OrderLogisticsInfo LogisticsInfo { get; set; }

        /// <summary>
        /// 备注信息，消费者下单时填写的订单备注信息，长度不超过2000字符
        /// </summary>
        [XmlElement("memo")]
        public string Memo { get; set; }

        /// <summary>
        /// 到位业务订单号。用户在到位下单时，由到位系统生成的32位全局唯一数字 id。  通过应用中的应用网关post发送给商户（应用网关配置参考链接：https%3A%2F%2Fdoc.open.alipay.com%2Fdocs%2Fdoc.htm%3Fspm%3Da219a.7629140.0.0.TcIuKL%26treeId%3D193%26articleId%3D105310%26docType%3D1）。
        /// </summary>
        [XmlElement("order_no")]
        public string OrderNo { get; set; }

        /// <summary>
        /// 到位订单状态枚举值，用于描述订单的业务状态，到位系统定义的枚举值（枚举：WAIT_CONFIRM：待接单；WAIT_ASSIGN_SP：待确认服务者；WAIT_SERVICE：待服务；SERVICE_START：服务者开始服务；CONFIRMED_SERVICE：服务者确认服务完成；SERVICE_COMPLETE：消费者确认服务完成；ORDER_FINISHED：订单正常结束；ORDER_CLOSE：订单中途关闭；
        /// </summary>
        [XmlElement("order_status")]
        public string OrderStatus { get; set; }

        /// <summary>
        /// 用户下订单后已付款金额，不小于0的数，单位为元，单个订单金额小于10w。
        /// </summary>
        [XmlElement("payment_amount")]
        public string PaymentAmount { get; set; }

        /// <summary>
        /// 用户下单产生的订单实际金额，不小于0的数，单位为元，单个订单金额小于10w。
        /// </summary>
        [XmlElement("real_amount")]
        public string RealAmount { get; set; }

        /// <summary>
        /// 订单已退款的金额，单位为元，若订单存在退款，则金额大于0，且小于等于实际支付的金额
        /// </summary>
        [XmlElement("refund_amount")]
        public string RefundAmount { get; set; }

        /// <summary>
        /// 服务订单列表：包含订单所对应的服务，服务可能包含不止一个，每个服务对应自身的单价、总价、退款价格等
        /// </summary>
        [XmlArray("service_order_list")]
        [XmlArrayItem("service_order_info")]
        public List<ServiceOrderInfo> ServiceOrderList { get; set; }

        /// <summary>
        /// 用户下单产生的订单总金额，不小于0的数，单位为元，单个订单金额小于10w
        /// </summary>
        [XmlElement("total_amount")]
        public string TotalAmount { get; set; }
    }
}

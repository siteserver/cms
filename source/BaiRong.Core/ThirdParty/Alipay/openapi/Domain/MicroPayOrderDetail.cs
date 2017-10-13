using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// MicroPayOrderDetail Data Structure.
    /// </summary>
    [Serializable]
    public class MicroPayOrderDetail : AopObject
    {
        /// <summary>
        /// 支付宝订单号，此订单号作为后续支付冻结金以及转账的订单标识
        /// </summary>
        [XmlElement("alipay_order_no")]
        public string AlipayOrderNo { get; set; }

        /// <summary>
        /// 支付方的支付宝userId
        /// </summary>
        [XmlElement("alipay_user_id")]
        public string AlipayUserId { get; set; }

        /// <summary>
        /// 可用于支付的金额（除去服务费）
        /// </summary>
        [XmlElement("available_amount")]
        public string AvailableAmount { get; set; }

        /// <summary>
        /// 订单创建时间
        /// </summary>
        [XmlElement("create_time")]
        public string CreateTime { get; set; }

        /// <summary>
        /// 冻结资金的到期自动解冻时间
        /// </summary>
        [XmlElement("expire_time")]
        public string ExpireTime { get; set; }

        /// <summary>
        /// 冻结金额（不包含服务费）
        /// </summary>
        [XmlElement("freeze_amount")]
        public string FreezeAmount { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [XmlElement("memo")]
        public string Memo { get; set; }

        /// <summary>
        /// 商户订单号,这个是调用传入的
        /// </summary>
        [XmlElement("merchant_order_no")]
        public string MerchantOrderNo { get; set; }

        /// <summary>
        /// 订单的最近修改时间
        /// </summary>
        [XmlElement("modified_time")]
        public string ModifiedTime { get; set; }

        /// <summary>
        /// 订单状态：I：初始，S：成功
        /// </summary>
        [XmlElement("order_status")]
        public string OrderStatus { get; set; }

        /// <summary>
        /// 支付的金额（含服务费）
        /// </summary>
        [XmlElement("pay_amount")]
        public string PayAmount { get; set; }

        /// <summary>
        /// 冻结资金支付确认方式， NO_CONFIRM：不需要付款确认，调用接口直接扣款 PAY_PASSWORD: 在转账需要付款方用支付密码确认，才可以转账成功
        /// </summary>
        [XmlElement("pay_confirm")]
        public string PayConfirm { get; set; }
    }
}

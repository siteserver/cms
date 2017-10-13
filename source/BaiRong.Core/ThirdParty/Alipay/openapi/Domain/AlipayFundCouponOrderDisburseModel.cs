using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayFundCouponOrderDisburseModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayFundCouponOrderDisburseModel : AopObject
    {
        /// <summary>
        /// 需要支付的金额，单位为：元（人民币），精确到小数点后两位  取值范围：[0.01,100000000.00]
        /// </summary>
        [XmlElement("amount")]
        public string Amount { get; set; }

        /// <summary>
        /// 发放红包时产生的支付宝资金订单号。deduct_auth_no与下面的deduct_out_order_no不能同时为空，二者都存在时，以deduct_auth_no为准。为了保证支付的高效，建议商户传入deduct_auth_no。
        /// </summary>
        [XmlElement("deduct_auth_no")]
        public string DeductAuthNo { get; set; }

        /// <summary>
        /// 发放红包时的商户授权资金订单号。deduct_out_order_no与上面的deduct_auth_no不能同时为空，二者都存在时，以deduct_auth_no为准。为了保证支付的高效，建议商户传入deduct_auth_no。
        /// </summary>
        [XmlElement("deduct_out_order_no")]
        public string DeductOutOrderNo { get; set; }

        /// <summary>
        /// 业务扩展参数，用于商户的特定业务信息的传递，json格式
        /// </summary>
        [XmlElement("extra_param")]
        public string ExtraParam { get; set; }

        /// <summary>
        /// 业务订单的简单描述，如商品名称等  长度不超过100个字母或50个汉字
        /// </summary>
        [XmlElement("order_title")]
        public string OrderTitle { get; set; }

        /// <summary>
        /// 商户的授权资金订单号  同一商户不同的订单，商户授权资金订单号不能重复
        /// </summary>
        [XmlElement("out_order_no")]
        public string OutOrderNo { get; set; }

        /// <summary>
        /// 商户本次资金操作的请求流水号  同一商户每次不同的资金操作请求，商户请求流水号不要重复
        /// </summary>
        [XmlElement("out_request_no")]
        public string OutRequestNo { get; set; }

        /// <summary>
        /// 该笔订单允许的最晚付款时间，逾期将关闭该笔订单  取值范围：1m～7d。m-分钟，h-小时，d-天。 该参数数值不接受小数点， 如 1.5h，可转换为90m，如果为空，默认1h
        /// </summary>
        [XmlElement("pay_timeout")]
        public string PayTimeout { get; set; }

        /// <summary>
        /// 收款方的支付宝登录号，形式为手机号或邮箱等
        /// </summary>
        [XmlElement("payee_logon_id")]
        public string PayeeLogonId { get; set; }

        /// <summary>
        /// 收款方的支付宝唯一用户号,以2088开头的16位纯数字组成
        /// </summary>
        [XmlElement("payee_user_id")]
        public string PayeeUserId { get; set; }
    }
}

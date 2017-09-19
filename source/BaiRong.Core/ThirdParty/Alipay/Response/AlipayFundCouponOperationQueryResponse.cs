using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayFundCouponOperationQueryResponse.
    /// </summary>
    public class AlipayFundCouponOperationQueryResponse : AopResponse
    {
        /// <summary>
        /// 该支付宝操作流水操作金额，单位为：元（人民币）
        /// </summary>
        [XmlElement("amount")]
        public string Amount { get; set; }

        /// <summary>
        /// 支付宝资金授权订单号
        /// </summary>
        [XmlElement("auth_no")]
        public string AuthNo { get; set; }

        /// <summary>
        /// 商户请求创建预授权订单时传入的扩展参数（merchantExt, orderExpiredTime）
        /// </summary>
        [XmlElement("extra_param")]
        public string ExtraParam { get; set; }

        /// <summary>
        /// 资金授权单据操作流水创建时间，  格式：YYYY-MM-DD HH:MM:SS
        /// </summary>
        [XmlElement("gmt_create")]
        public string GmtCreate { get; set; }

        /// <summary>
        /// 支付宝账务处理成功时间，  格式：YYYY-MM-DD HH:MM:SS
        /// </summary>
        [XmlElement("gmt_trans")]
        public string GmtTrans { get; set; }

        /// <summary>
        /// 支付宝资金操作流水号
        /// </summary>
        [XmlElement("operation_id")]
        public string OperationId { get; set; }

        /// <summary>
        /// 支付宝资金操作类型。  目前支持：   PAY：支付  REFUND：退款
        /// </summary>
        [XmlElement("operation_type")]
        public string OperationType { get; set; }

        /// <summary>
        /// 业务订单的简单描述，如商品名称等
        /// </summary>
        [XmlElement("order_title")]
        public string OrderTitle { get; set; }

        /// <summary>
        /// 商户的授权资金订单号
        /// </summary>
        [XmlElement("out_order_no")]
        public string OutOrderNo { get; set; }

        /// <summary>
        /// 商户资金操作的请求流水号
        /// </summary>
        [XmlElement("out_request_no")]
        public string OutRequestNo { get; set; }

        /// <summary>
        /// 收款方支付宝账号（Email或手机号）。仅作展示使用，默认会加“*”号处理
        /// </summary>
        [XmlElement("payee_logon_id")]
        public string PayeeLogonId { get; set; }

        /// <summary>
        /// 收款方支付宝账号对应的支付宝唯一用户号。  以2088开头的16位纯数字组成。
        /// </summary>
        [XmlElement("payee_user_id")]
        public string PayeeUserId { get; set; }

        /// <summary>
        /// 付款方支付宝账号（Email或手机号）。仅作展示使用，默认会加“*”号处理
        /// </summary>
        [XmlElement("payer_logon_id")]
        public string PayerLogonId { get; set; }

        /// <summary>
        /// 付款方支付宝账号对应的支付宝唯一用户号。  以2088开头的16位纯数字组成。
        /// </summary>
        [XmlElement("payer_user_id")]
        public string PayerUserId { get; set; }

        /// <summary>
        /// 商户对本次操作的附言描述，长度不超过100个字母或50个汉字
        /// </summary>
        [XmlElement("remark")]
        public string Remark { get; set; }

        /// <summary>
        /// 资金操作流水的状态，  目前支持：   INIT：初始  SUCCESS：成功  CLOSED：关闭
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }

        /// <summary>
        /// 红包总退回金额，单位为：元（人民币）
        /// </summary>
        [XmlElement("total_pay_refund_amount")]
        public string TotalPayRefundAmount { get; set; }
    }
}

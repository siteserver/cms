using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayTradePayConsultModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayTradePayConsultModel : AopObject
    {
        /// <summary>
        /// 支付宝系统中用以唯一标识用户签约记录的编号。用户签约成功后时，协议号会返回给商户。
        /// </summary>
        [XmlElement("agreement_no")]
        public string AgreementNo { get; set; }

        /// <summary>
        /// 商户申请额度，商户端根据实际需要来赋值。
        /// </summary>
        [XmlElement("apply_amount")]
        public string ApplyAmount { get; set; }

        /// <summary>
        /// 业务场景，用于区分商户具体的咨询类型。ENJOY_CONSULT：兜底资金咨询；FUND_BILL_CONSULT资金渠道咨询
        /// </summary>
        [XmlElement("biz_scene")]
        public string BizScene { get; set; }

        /// <summary>
        /// 买家的支付宝用户id，用户签约成功后，会返回给商户。
        /// </summary>
        [XmlElement("buyer_id")]
        public string BuyerId { get; set; }

        /// <summary>
        /// 支付咨询阶段。在支付过程中，用于区分商户发起咨询的阶段。BEFORE_PAY：支付前咨询；AFTER_PAY：支付后咨询
        /// </summary>
        [XmlElement("consult_phase")]
        public string ConsultPhase { get; set; }

        /// <summary>
        /// 扩展参数，必须是json格式
        /// </summary>
        [XmlElement("extend_params")]
        public string ExtendParams { get; set; }

        /// <summary>
        /// 此参数值取商户签约销售方案时的销售产品码
        /// </summary>
        [XmlElement("product_code")]
        public string ProductCode { get; set; }

        /// <summary>
        /// 商户端生成唯一标识，64个字符以内、可包含字母、数字、下划线；需保证在商户端不重复
        /// </summary>
        [XmlElement("request_no")]
        public string RequestNo { get; set; }

        /// <summary>
        /// 订单标题，商户端描述该次咨询对应的基本订单信息。
        /// </summary>
        [XmlElement("subject")]
        public string Subject { get; set; }
    }
}

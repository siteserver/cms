using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayOfflineMarketShopCreateResponse.
    /// </summary>
    public class AlipayOfflineMarketShopCreateResponse : AopResponse
    {
        /// <summary>
        /// 开店请求受理成功后返回的支付宝流水ID，根据此ID调用接口  alipay.offline.market.applyorder.batchquery，能够获取当前开店请求审核状态、驳回原因等信息。
        /// </summary>
        [XmlElement("apply_id")]
        public string ApplyId { get; set; }

        /// <summary>
        /// 废弃字段。
        /// </summary>
        [XmlElement("audit_desc")]
        public string AuditDesc { get; set; }

        /// <summary>
        /// 同步请求如果支付宝受理成功，将返回AUDITING状态。
        /// </summary>
        [XmlElement("audit_status")]
        public string AuditStatus { get; set; }

        /// <summary>
        /// 废弃字段，T表示上架,F表示未上架。
        /// </summary>
        [XmlElement("is_online")]
        public string IsOnline { get; set; }

        /// <summary>
        /// 废弃字段，T表示显示，F表示隐藏。
        /// </summary>
        [XmlElement("is_show")]
        public string IsShow { get; set; }

        /// <summary>
        /// 开店请求结果码：  WAIT_MERCHANT_CONFIRM：等待商户确认  当开店需要商户确认时返回此结果码，商户需要登录到商家中心e.alipay.com进行开店确认。例如，ISV帮商户开店时，出现需要改签口碑当面付主体，接口同步返回此错误码，并且支付宝发送短信告知商户登录商家中心进行改签确认，确认后进入到后续审核流程。
        /// </summary>
        [XmlElement("result_code")]
        public string ResultCode { get; set; }

        /// <summary>
        /// 废弃字段，支付宝门店ID。
        /// </summary>
        [XmlElement("shop_id")]
        public string ShopId { get; set; }
    }
}

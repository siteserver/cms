using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// Coupon Data Structure.
    /// </summary>
    [Serializable]
    public class Coupon : AopObject
    {
        /// <summary>
        /// 当前可用面额
        /// </summary>
        [XmlElement("available_amount")]
        public string AvailableAmount { get; set; }

        /// <summary>
        /// 红包编号
        /// </summary>
        [XmlElement("coupon_no")]
        public string CouponNo { get; set; }

        /// <summary>
        /// 可优惠面额
        /// </summary>
        [XmlElement("deduct_amount")]
        public string DeductAmount { get; set; }

        /// <summary>
        /// 有效期开始时间
        /// </summary>
        [XmlElement("gmt_active")]
        public string GmtActive { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [XmlElement("gmt_create")]
        public string GmtCreate { get; set; }

        /// <summary>
        /// 有效期结束时间
        /// </summary>
        [XmlElement("gmt_expired")]
        public string GmtExpired { get; set; }

        /// <summary>
        /// 红包使用说明
        /// </summary>
        [XmlElement("instructions")]
        public string Instructions { get; set; }

        /// <summary>
        /// 红包详情说明
        /// </summary>
        [XmlElement("memo")]
        public string Memo { get; set; }

        /// <summary>
        /// 商户id
        /// </summary>
        [XmlElement("merchant_uniq_id")]
        public string MerchantUniqId { get; set; }

        /// <summary>
        /// 是否可叠加
        /// </summary>
        [XmlElement("multi_use_flag")]
        public string MultiUseFlag { get; set; }

        /// <summary>
        /// 红包名称
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// 是否可退款标识
        /// </summary>
        [XmlElement("refund_flag")]
        public string RefundFlag { get; set; }

        /// <summary>
        /// 红包状态信息
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }

        /// <summary>
        /// 红包模板编号
        /// </summary>
        [XmlElement("template_no")]
        public string TemplateNo { get; set; }

        /// <summary>
        /// 用户openid
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}

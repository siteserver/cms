using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KbOrderVoucherModel Data Structure.
    /// </summary>
    [Serializable]
    public class KbOrderVoucherModel : AopObject
    {
        /// <summary>
        /// 商品凭证过期时间
        /// </summary>
        [XmlElement("expire_date")]
        public string ExpireDate { get; set; }

        /// <summary>
        /// 商品凭证核销／退款对应的资金流水号
        /// </summary>
        [XmlElement("funds_voucher_no")]
        public string FundsVoucherNo { get; set; }

        /// <summary>
        /// 商品ID
        /// </summary>
        [XmlElement("item_id")]
        public string ItemId { get; set; }

        /// <summary>
        /// 退款理由，由消费者选择或填写内容，系统退款可以为空。
        /// </summary>
        [XmlElement("refund_reason")]
        public string RefundReason { get; set; }

        /// <summary>
        /// 退款类型，ROLE_DAEMON（超期未使用），ROLE_USER（消费者主动）；
        /// </summary>
        [XmlElement("refund_type")]
        public string RefundType { get; set; }

        /// <summary>
        /// 商品凭证核销门店ID,核销后会存在该字段
        /// </summary>
        [XmlElement("shop_id")]
        public string ShopId { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }

        /// <summary>
        /// 商品凭证核销门店外部ID
        /// </summary>
        [XmlElement("store_id")]
        public string StoreId { get; set; }

        /// <summary>
        /// 商品凭证ID
        /// </summary>
        [XmlElement("voucher_id")]
        public string VoucherId { get; set; }
    }
}

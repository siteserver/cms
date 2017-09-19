using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// VoucherDetail Data Structure.
    /// </summary>
    [Serializable]
    public class VoucherDetail : AopObject
    {
        /// <summary>
        /// 优惠券面额，它应该会等于商家出资加上其他出资方出资
        /// </summary>
        [XmlElement("amount")]
        public string Amount { get; set; }

        /// <summary>
        /// 券id
        /// </summary>
        [XmlElement("id")]
        public string Id { get; set; }

        /// <summary>
        /// 优惠券备注信息
        /// </summary>
        [XmlElement("memo")]
        public string Memo { get; set; }

        /// <summary>
        /// 商家出资（特指发起交易的商家出资金额）
        /// </summary>
        [XmlElement("merchant_contribute")]
        public string MerchantContribute { get; set; }

        /// <summary>
        /// 券名称
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// 其他出资方出资金额，可能是支付宝，可能是品牌商，或者其他方，也可能是他们的一起出资
        /// </summary>
        [XmlElement("other_contribute")]
        public string OtherContribute { get; set; }

        /// <summary>
        /// 如果使用的这张券是用户购买的，则该字段代表用户在购买这张券时平台优惠的金额
        /// </summary>
        [XmlElement("purchase_ant_contribute")]
        public string PurchaseAntContribute { get; set; }

        /// <summary>
        /// 如果使用的这张券是用户购买的，则该字段代表用户在购买这张券时用户实际付款的金额
        /// </summary>
        [XmlElement("purchase_buyer_contribute")]
        public string PurchaseBuyerContribute { get; set; }

        /// <summary>
        /// 如果使用的这张券是用户购买的，则该字段代表用户在购买这张券时商户优惠的金额
        /// </summary>
        [XmlElement("purchase_merchant_contribute")]
        public string PurchaseMerchantContribute { get; set; }

        /// <summary>
        /// 当前有三种类型：  ALIPAY_FIX_VOUCHER - 全场代金券  ALIPAY_DISCOUNT_VOUCHER - 折扣券  ALIPAY_ITEM_VOUCHER - 单品优惠  注：不排除将来新增其他类型的可能，商家接入时注意兼容性避免硬编码
        /// </summary>
        [XmlElement("type")]
        public string Type { get; set; }
    }
}

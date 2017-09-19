using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// KoubeiTradeTicketTicketcodeUseResponse.
    /// </summary>
    public class KoubeiTradeTicketTicketcodeUseResponse : AopResponse
    {
        /// <summary>
        /// 用户购买券的时候实际支付的金额
        /// </summary>
        [XmlElement("buyer_pay_amount")]
        public string BuyerPayAmount { get; set; }

        /// <summary>
        /// 用户购买时商品的现价
        /// </summary>
        [XmlElement("current_price")]
        public string CurrentPrice { get; set; }

        /// <summary>
        /// 优惠金额，含商家补贴金额和口碑补贴金额
        /// </summary>
        [XmlElement("discount_amount")]
        public string DiscountAmount { get; set; }

        /// <summary>
        /// 交易中可给用户开具发票的金额
        /// </summary>
        [XmlElement("invoice_amount")]
        public string InvoiceAmount { get; set; }

        /// <summary>
        /// 口碑商品ID
        /// </summary>
        [XmlElement("item_id")]
        public string ItemId { get; set; }

        /// <summary>
        /// 用户购买时商品的名称
        /// </summary>
        [XmlElement("item_name")]
        public string ItemName { get; set; }

        /// <summary>
        /// 口碑补贴金额
        /// </summary>
        [XmlElement("koubei_subsidy_amount")]
        public string KoubeiSubsidyAmount { get; set; }

        /// <summary>
        /// 券码所属的订单id
        /// </summary>
        [XmlElement("order_no")]
        public string OrderNo { get; set; }

        /// <summary>
        /// 用户购买时商品的原价
        /// </summary>
        [XmlElement("original_price")]
        public string OriginalPrice { get; set; }

        /// <summary>
        /// 商家实收金额
        /// </summary>
        [XmlElement("receipt_amount")]
        public string ReceiptAmount { get; set; }

        /// <summary>
        /// 外部请求号，支持英文字母和数字，由开发者自行定义（不允许重复）
        /// </summary>
        [XmlElement("request_id")]
        public string RequestId { get; set; }

        /// <summary>
        /// 12位的券码，券码为纯数字，且唯一不重复
        /// </summary>
        [XmlElement("ticket_code")]
        public string TicketCode { get; set; }

        /// <summary>
        /// 核销流水号
        /// </summary>
        [XmlElement("ticket_trans_id")]
        public string TicketTransId { get; set; }

        /// <summary>
        /// 券核销时间
        /// </summary>
        [XmlElement("use_date")]
        public string UseDate { get; set; }

        /// <summary>
        /// 券核销门店ID
        /// </summary>
        [XmlElement("use_shop_id")]
        public string UseShopId { get; set; }

        /// <summary>
        /// 券核销门店名称
        /// </summary>
        [XmlElement("use_shop_name")]
        public string UseShopName { get; set; }

        /// <summary>
        /// 券码对应的凭证资产id
        /// </summary>
        [XmlElement("voucher_id")]
        public string VoucherId { get; set; }
    }
}

using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ZhimaMerchantOrderRentCompleteModel Data Structure.
    /// </summary>
    [Serializable]
    public class ZhimaMerchantOrderRentCompleteModel : AopObject
    {
        /// <summary>
        /// 信用借还订单号
        /// </summary>
        [XmlElement("order_no")]
        public string OrderNo { get; set; }

        /// <summary>
        /// 支付金额
        /// </summary>
        [XmlElement("pay_amount")]
        public string PayAmount { get; set; }

        /// <summary>
        /// 金额类型：  RENT:租金  DAMAGE:赔偿金
        /// </summary>
        [XmlElement("pay_amount_type")]
        public string PayAmountType { get; set; }

        /// <summary>
        /// 信用借还的产品码:w1010100000000002858
        /// </summary>
        [XmlElement("product_code")]
        public string ProductCode { get; set; }

        /// <summary>
        /// 物品归还门店名称
        /// </summary>
        [XmlElement("restore_shop_name")]
        public string RestoreShopName { get; set; }

        /// <summary>
        /// 物品归还时间
        /// </summary>
        [XmlElement("restore_time")]
        public string RestoreTime { get; set; }
    }
}

using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ReduceInfo Data Structure.
    /// </summary>
    [Serializable]
    public class ReduceInfo : AopObject
    {
        /// <summary>
        /// 门店品牌名称
        /// </summary>
        [XmlElement("brand_name")]
        public string BrandName { get; set; }

        /// <summary>
        /// 消费金额（单位分）
        /// </summary>
        [XmlElement("consume_amt")]
        public string ConsumeAmt { get; set; }

        /// <summary>
        /// 消费门店名称
        /// </summary>
        [XmlElement("consume_store_name")]
        public string ConsumeStoreName { get; set; }

        /// <summary>
        /// 消费时间
        /// </summary>
        [XmlElement("payment_time")]
        public string PaymentTime { get; set; }

        /// <summary>
        /// 优惠金额（单位分）
        /// </summary>
        [XmlElement("promo_amt")]
        public string PromoAmt { get; set; }

        /// <summary>
        /// 用户名（脱敏）
        /// </summary>
        [XmlElement("user_name")]
        public string UserName { get; set; }
    }
}

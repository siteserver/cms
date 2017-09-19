using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// PromoItemInfo Data Structure.
    /// </summary>
    [Serializable]
    public class PromoItemInfo : AopObject
    {
        /// <summary>
        /// 商品购买限制
        /// </summary>
        [XmlElement("constraint_info")]
        public ConstraintInfo ConstraintInfo { get; set; }

        /// <summary>
        /// 商品ID，用于领取时使用
        /// </summary>
        [XmlElement("id")]
        public string Id { get; set; }

        /// <summary>
        /// 商品售卖结束时间
        /// </summary>
        [XmlElement("sale_end_time")]
        public string SaleEndTime { get; set; }

        /// <summary>
        /// 商品售卖的开始时间
        /// </summary>
        [XmlElement("sale_start_time")]
        public string SaleStartTime { get; set; }

        /// <summary>
        /// 剩余库存
        /// </summary>
        [XmlElement("total_inventory")]
        public string TotalInventory { get; set; }

        /// <summary>
        /// 券信息
        /// </summary>
        [XmlElement("voucher")]
        public Voucher Voucher { get; set; }
    }
}

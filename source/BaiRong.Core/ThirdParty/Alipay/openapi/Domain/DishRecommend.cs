using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// DishRecommend Data Structure.
    /// </summary>
    [Serializable]
    public class DishRecommend : AopObject
    {
        /// <summary>
        /// 购买可能性/商品热度得分
        /// </summary>
        [XmlElement("buy_posibility")]
        public long BuyPosibility { get; set; }

        /// <summary>
        /// 菜品ID
        /// </summary>
        [XmlElement("dish_id")]
        public string DishId { get; set; }

        /// <summary>
        /// 菜品名称
        /// </summary>
        [XmlElement("dish_name")]
        public string DishName { get; set; }
    }
}

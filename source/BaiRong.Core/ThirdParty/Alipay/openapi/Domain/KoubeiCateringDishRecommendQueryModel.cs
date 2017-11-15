using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiCateringDishRecommendQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiCateringDishRecommendQueryModel : AopObject
    {
        /// <summary>
        /// 用户已点的主菜品ID，传入时作为推荐菜品参考
        /// </summary>
        [XmlElement("dish_id")]
        public string DishId { get; set; }

        /// <summary>
        /// 点餐门店所属的商家PID
        /// </summary>
        [XmlElement("merchent_pid")]
        public string MerchentPid { get; set; }

        /// <summary>
        /// 点餐的门店ID
        /// </summary>
        [XmlElement("shop_id")]
        public string ShopId { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}

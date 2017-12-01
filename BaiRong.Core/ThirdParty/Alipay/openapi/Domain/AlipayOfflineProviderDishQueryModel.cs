using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayOfflineProviderDishQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayOfflineProviderDishQueryModel : AopObject
    {
        /// <summary>
        /// 数据是根据alipay.offline.provider.shopaction.record的插入菜品接口获取，对应字段是：dishTypeName。
        /// </summary>
        [XmlElement("dish_type_name")]
        public string DishTypeName { get; set; }

        /// <summary>
        /// order_by：1，菜品热度升序查询，order_by：2，菜品热度降序查询。不设置时默认为2(菜品热度降序查询)
        /// </summary>
        [XmlElement("order_by")]
        public string OrderBy { get; set; }

        /// <summary>
        /// ISV自己的菜品ID，数据的计算根据：alipay.offline.provider.shopaction.record接口中插入菜品与alipay.offline.provider.useraction.record上传用户点菜菜单作为元数据，通过分析得到的数据。当前的ID就是插入菜品中的outerDishId，同时也是上传用户点菜中的action_type是order_dishes里面的dish对象的goodsId
        /// </summary>
        [XmlElement("outer_dish_id")]
        public string OuterDishId { get; set; }

        /// <summary>
        /// 需要查询的第几页信息。非必填。默认为1
        /// </summary>
        [XmlElement("page")]
        public long Page { get; set; }

        /// <summary>
        /// 分页查询每页的条数，默认为20条，每次最大拉去条数100,超过限制直接返回错误
        /// </summary>
        [XmlElement("page_size")]
        public long PageSize { get; set; }

        /// <summary>
        /// 口碑店铺id，商户订购开发者服务插件后，口碑会通过服务市场管理推送订购信息给开发者，开发者可通过其中的订购插件订单明细查询获取此参数值，或通过商户授权口碑开店接口来获取。
        /// </summary>
        [XmlElement("shop_id")]
        public string ShopId { get; set; }
    }
}

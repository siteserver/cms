using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// IsvShopDishModel Data Structure.
    /// </summary>
    [Serializable]
    public class IsvShopDishModel : AopObject
    {
        /// <summary>
        /// 菜品库存。 alipay.offline.provider.shopaction.record回传点菜中的desc。建议ISV在拿到推荐的菜品的ID后，直接使用自己的菜品元数据，口碑元数据是ISV上传，实时性无法保证。
        /// </summary>
        [XmlElement("content")]
        public string Content { get; set; }

        /// <summary>
        /// 菜品分类ID  alipay.offline.provider.shopaction.record回传点菜中的dishTypeID，建议ISV在拿到推荐的菜品的ID后，直接使用自己的菜品元数据，口碑元数据是ISV上传，实时性无法保证。
        /// </summary>
        [XmlElement("dish_type_id")]
        public string DishTypeId { get; set; }

        /// <summary>
        /// 商家定义菜品的分类名称 alipay.offline.provider.shopaction.record回传点菜中的dishTypeName，建议ISV在拿到推荐的菜品的ID后，直接使用自己的菜品元数据，口碑元数据是ISV上传，实时性无法保证。
        /// </summary>
        [XmlElement("dish_type_name")]
        public string DishTypeName { get; set; }

        /// <summary>
        /// 菜品热度等级（0/0.5/1/1.5/2/2.5/3/3.5/4/4.5/5）该字段是对sort_col做离散化，数字越大越热
        /// </summary>
        [XmlElement("good_level")]
        public string GoodLevel { get; set; }

        /// <summary>
        /// 当前店铺的商家最近7天销量（份）
        /// </summary>
        [XmlElement("merchant_sold_cnt_seven_d")]
        public long MerchantSoldCntSevenD { get; set; }

        /// <summary>
        /// 当前店铺的商家最近30天销量（份）
        /// </summary>
        [XmlElement("merchant_sold_cnt_thirty_d")]
        public long MerchantSoldCntThirtyD { get; set; }

        /// <summary>
        /// 当前店铺的商家最近30天购买2次及以上的支付宝用户数
        /// </summary>
        [XmlElement("merchant_sold_reusercnt_thirty_d")]
        public long MerchantSoldReusercntThirtyD { get; set; }

        /// <summary>
        /// 当前店铺的商家最近30天消费支付宝用户数
        /// </summary>
        [XmlElement("merchant_sold_usercnt_thirty_d")]
        public long MerchantSoldUsercntThirtyD { get; set; }

        /// <summary>
        /// alipay.offline.provider.shopaction.record回传点菜中的name
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// ISV自己的菜品ID，数据的计算根据：alipay.offline.provider.shopaction.record接口中插入菜品与alipay.offline.provider.useraction.record上传用户点菜菜单作为元数据，通过分析得到的数据。当前的ID就是插入菜品中的outerDishId，同时也是上传用户点菜中的action_type是order_dishes里面的dish对象的goodsId
        /// </summary>
        [XmlElement("outer_dish_id")]
        public string OuterDishId { get; set; }

        /// <summary>
        /// 废弃，请ISV使用自己的图
        /// </summary>
        [XmlElement("pict")]
        public string Pict { get; set; }

        /// <summary>
        /// 当前值来自于alipay.offline.provider.shopaction.record中的outer_shop_do对象里面的 type字段。
        /// </summary>
        [XmlElement("platform")]
        public string Platform { get; set; }

        /// <summary>
        /// alipay.offline.provider.shopaction.record回传点菜中的price，建议ISV在拿到推荐的菜品的ID后，直接使用自己的菜品元数据，口碑元数据是ISV上传，实时性无法保证。
        /// </summary>
        [XmlElement("price")]
        public string Price { get; set; }

        /// <summary>
        /// 菜品库存。  alipay.offline.provider.shopaction.record回传点菜中的quantity，建议ISV在拿到推荐的菜品的ID后，直接使用自己的菜品元数据，口碑元数据是ISV上传，实时性无法保证。
        /// </summary>
        [XmlElement("quantity")]
        public long Quantity { get; set; }

        /// <summary>
        /// 口碑店铺id，商户订购开发者服务插件后，口碑会通过服务市场管理推送订购信息给开发者，开发者可通过其中的订购插件订单明细查询获取此参数值，或通过商户授权口碑开店接口来获取。
        /// </summary>
        [XmlElement("shop_id")]
        public string ShopId { get; set; }

        /// <summary>
        /// 当前店铺最近7天销量（份）
        /// </summary>
        [XmlElement("sold_cnt_seven_d")]
        public long SoldCntSevenD { get; set; }

        /// <summary>
        /// 当前店铺最近30天销量（份）
        /// </summary>
        [XmlElement("sold_cnt_thirty_d")]
        public long SoldCntThirtyD { get; set; }

        /// <summary>
        /// 当前店铺最近30天购买2次及以上的支付宝用户数
        /// </summary>
        [XmlElement("sold_reusercnt_thirty_d")]
        public long SoldReusercntThirtyD { get; set; }

        /// <summary>
        /// 当前店铺最近30天消费支付宝用户数
        /// </summary>
        [XmlElement("sold_usercnt_thirty_d")]
        public long SoldUsercntThirtyD { get; set; }

        /// <summary>
        /// 排序值。 alipay.offline.provider.shopaction.record回传点菜中的sort。建议ISV在拿到推荐的菜品的ID后，直接使用自己的菜品元数据，口碑元数据是ISV上传，实时性无法保证。
        /// </summary>
        [XmlArray("sort_col")]
        [XmlArrayItem("number")]
        public List<long> SortCol { get; set; }

        /// <summary>
        /// 菜品显示的单位（份/斤/杯） alipay.offline.provider.shopaction.record回传点菜中的unit，建议ISV在拿到推荐的菜品的ID后，直接使用自己的菜品元数据，口碑元数据是ISV上传，实时性无法保证。
        /// </summary>
        [XmlElement("unit")]
        public string Unit { get; set; }
    }
}

using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayDataDataserviceShoppingmallrecShopQueryResponse.
    /// </summary>
    public class AlipayDataDataserviceShoppingmallrecShopQueryResponse : AopResponse
    {
        /// <summary>
        /// 本次推荐的id, 开发者埋点需带入此参数
        /// </summary>
        [XmlElement("recommend_id")]
        public string RecommendId { get; set; }

        /// <summary>
        /// 本次请求的全局唯一标识, 支持英文字母和数字, 由开发者自行定义,和入参request_id一致
        /// </summary>
        [XmlElement("request_id")]
        public string RequestId { get; set; }

        /// <summary>
        /// 推荐的商铺列表, 有序
        /// </summary>
        [XmlArray("shop_recommend_list")]
        [XmlArrayItem("shop_rec")]
        public List<ShopRec> ShopRecommendList { get; set; }
    }
}

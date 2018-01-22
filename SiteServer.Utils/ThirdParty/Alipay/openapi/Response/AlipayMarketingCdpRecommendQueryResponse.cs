using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayMarketingCdpRecommendQueryResponse.
    /// </summary>
    public class AlipayMarketingCdpRecommendQueryResponse : AopResponse
    {
        /// <summary>
        /// 当前推荐的唯一标识,用于不同方案的效果跟踪
        /// </summary>
        [XmlElement("recommend_id")]
        public string RecommendId { get; set; }

        /// <summary>
        /// 商家信息列表，最多返回20条，返回json数组，包含shopTitle=店名；cuisine=菜系；address=具体位置；bizCircle=所属商圈；discount=优惠；orderCount=已领/已享人数；avgPrice=人均价格；starLevel=评分；distance=距离（单位米）。以上属性可以为空或不存在
        /// </summary>
        [XmlElement("shop_info")]
        public string ShopInfo { get; set; }
    }
}

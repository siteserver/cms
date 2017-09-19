using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiAdvertDeliveryItemApplyModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiAdvertDeliveryItemApplyModel : AopObject
    {
        /// <summary>
        /// 领券时触发的id，在外投场景下，用的是广告id
        /// </summary>
        [XmlElement("adv_id")]
        public string AdvId { get; set; }

        /// <summary>
        /// 渠道编号，适用于媒体类发券
        /// </summary>
        [XmlElement("channel_code")]
        public string ChannelCode { get; set; }

        /// <summary>
        /// 适用于在推广者主站上注册的渠道编号
        /// </summary>
        [XmlElement("channel_id")]
        public string ChannelId { get; set; }

        /// <summary>
        /// 外部流水号，用于区别每次请求的流水号
        /// </summary>
        [XmlElement("out_biz_no")]
        public string OutBizNo { get; set; }

        /// <summary>
        /// 券推荐时输出给合作伙伴的id，需要在领券的时候传回来
        /// </summary>
        [XmlElement("recommend_id")]
        public string RecommendId { get; set; }

        /// <summary>
        /// 领取优惠的门店id
        /// </summary>
        [XmlElement("shop_id")]
        public string ShopId { get; set; }

        /// <summary>
        /// 推广参与打标(无实际业务作用，后期可供ISV分析不同渠道的推广效能)
        /// </summary>
        [XmlElement("tag")]
        public string Tag { get; set; }
    }
}

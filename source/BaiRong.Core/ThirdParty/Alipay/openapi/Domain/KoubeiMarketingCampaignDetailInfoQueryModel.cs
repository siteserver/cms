using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiMarketingCampaignDetailInfoQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiMarketingCampaignDetailInfoQueryModel : AopObject
    {
        /// <summary>
        /// 营销活动id，配合《店铺优惠查询》接口使用，该接口返回camp_list列表中的biz_id则对应该id。
        /// </summary>
        [XmlElement("camp_id")]
        public string CampId { get; set; }

        /// <summary>
        /// 适用门店限制返回数量
        /// </summary>
        [XmlElement("shop_limit_count")]
        public long ShopLimitCount { get; set; }
    }
}

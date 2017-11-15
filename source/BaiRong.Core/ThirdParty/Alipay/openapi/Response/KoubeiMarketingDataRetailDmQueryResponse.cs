using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// KoubeiMarketingDataRetailDmQueryResponse.
    /// </summary>
    public class KoubeiMarketingDataRetailDmQueryResponse : AopResponse
    {
        /// <summary>
        /// 内容ID
        /// </summary>
        [XmlElement("content_id")]
        public string ContentId { get; set; }

        /// <summary>
        /// 店面DM营销数据，包括商品的PV、UV、曝光。
        /// </summary>
        [XmlArray("dm_marketing_datas")]
        [XmlArrayItem("dm_activity_shop_data")]
        public List<DmActivityShopData> DmMarketingDatas { get; set; }

        /// <summary>
        /// 商品码
        /// </summary>
        [XmlElement("item_code")]
        public string ItemCode { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [XmlElement("item_name")]
        public string ItemName { get; set; }
    }
}

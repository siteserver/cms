using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// KoubeiMarketingCampaignActivityBatchqueryResponse.
    /// </summary>
    public class KoubeiMarketingCampaignActivityBatchqueryResponse : AopResponse
    {
        /// <summary>
        /// 活动列表
        /// </summary>
        [XmlArray("camp_sets")]
        [XmlArrayItem("camp_base_dto")]
        public List<CampBaseDto> CampSets { get; set; }

        /// <summary>
        /// 总数量
        /// </summary>
        [XmlElement("total_number")]
        public string TotalNumber { get; set; }
    }
}

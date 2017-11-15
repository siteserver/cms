using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// KoubeiMarketingCampaignCrowdCountResponse.
    /// </summary>
    public class KoubeiMarketingCampaignCrowdCountResponse : AopResponse
    {
        /// <summary>
        /// 各个细分维度的值，label为标签code，value为该标签各个标签值对应的统计信息，本示例表示pam_gender这个标签的男有100人，女有1000人满足入参指定的圈人条件
        /// </summary>
        [XmlElement("dimension_values")]
        public string DimensionValues { get; set; }

        /// <summary>
        /// 人群组的汇总统计值total是人数，sum是交易金额
        /// </summary>
        [XmlElement("summary_values")]
        public string SummaryValues { get; set; }
    }
}

using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// KoubeiMarketingCampaignActivityCreateResponse.
    /// </summary>
    public class KoubeiMarketingCampaignActivityCreateResponse : AopResponse
    {
        /// <summary>
        /// 活动审批状态，  仅限服务商代商户创建活动时返回  AUDITING，审核中,REJECT为驳回，不返回表示通过
        /// </summary>
        [XmlElement("audit_status")]
        public string AuditStatus { get; set; }

        /// <summary>
        /// 活动ID
        /// </summary>
        [XmlElement("camp_id")]
        public string CampId { get; set; }

        /// <summary>
        /// 活动状态，目前返回以下状态，  STARTING，活动启动中  STARTED，活动已启动
        /// </summary>
        [XmlElement("camp_status")]
        public string CampStatus { get; set; }
    }
}

using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KbadvertSmartPromoRequest Data Structure.
    /// </summary>
    [Serializable]
    public class KbadvertSmartPromoRequest : AopObject
    {
        /// <summary>
        /// 智能营销分组ID
        /// </summary>
        [XmlElement("group_id")]
        public string GroupId { get; set; }

        /// <summary>
        /// 智能营销方案ID
        /// </summary>
        [XmlElement("plan_id")]
        public string PlanId { get; set; }
    }
}

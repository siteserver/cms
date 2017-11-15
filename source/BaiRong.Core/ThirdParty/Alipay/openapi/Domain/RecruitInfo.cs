using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// RecruitInfo Data Structure.
    /// </summary>
    [Serializable]
    public class RecruitInfo : AopObject
    {
        /// <summary>
        /// 招商结束时间
        /// </summary>
        [XmlElement("end_time")]
        public string EndTime { get; set; }

        /// <summary>
        /// 是否参与门店参与了招商
        /// </summary>
        [XmlElement("exclude_constraint_shops")]
        public bool ExcludeConstraintShops { get; set; }

        /// <summary>
        /// 招商方案id
        /// </summary>
        [XmlElement("plan_id")]
        public string PlanId { get; set; }

        /// <summary>
        /// 招商开始时间
        /// </summary>
        [XmlElement("start_time")]
        public string StartTime { get; set; }

        /// <summary>
        /// 招商状态
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }
    }
}

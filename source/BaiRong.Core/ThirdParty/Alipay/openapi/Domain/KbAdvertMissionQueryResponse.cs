using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KbAdvertMissionQueryResponse Data Structure.
    /// </summary>
    [Serializable]
    public class KbAdvertMissionQueryResponse : AopObject
    {
        /// <summary>
        /// 任务结束时间
        /// </summary>
        [XmlElement("gmt_end")]
        public string GmtEnd { get; set; }

        /// <summary>
        /// 任务开始时间
        /// </summary>
        [XmlElement("gmt_start")]
        public string GmtStart { get; set; }

        /// <summary>
        /// 分佣任务ID
        /// </summary>
        [XmlElement("mission_id")]
        public string MissionId { get; set; }

        /// <summary>
        /// 推广状态  EFFECTIVE-有效  INVALID-无效
        /// </summary>
        [XmlElement("promote_status")]
        public string PromoteStatus { get; set; }

        /// <summary>
        /// 分佣标的信息
        /// </summary>
        [XmlArray("subjects")]
        [XmlArrayItem("kb_advert_mission_subject")]
        public List<KbAdvertMissionSubject> Subjects { get; set; }
    }
}

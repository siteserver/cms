using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// CraftsmanAssessment Data Structure.
    /// </summary>
    [Serializable]
    public class CraftsmanAssessment : AopObject
    {
        /// <summary>
        /// 子评分项
        /// </summary>
        [XmlArray("sub_assessments")]
        [XmlArrayItem("craftsman_sub_assessment")]
        public List<CraftsmanSubAssessment> SubAssessments { get; set; }

        /// <summary>
        /// 单个手艺人的评价总条数。
        /// </summary>
        [XmlElement("total_no")]
        public long TotalNo { get; set; }

        /// <summary>
        /// 单个手艺人的总评分的均分
        /// </summary>
        [XmlElement("total_score")]
        public long TotalScore { get; set; }
    }
}

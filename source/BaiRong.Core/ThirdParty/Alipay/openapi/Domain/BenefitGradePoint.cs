using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// BenefitGradePoint Data Structure.
    /// </summary>
    [Serializable]
    public class BenefitGradePoint : AopObject
    {
        /// <summary>
        /// 蚂蚁会员权益配置的ID
        /// </summary>
        [XmlElement("benefit_id")]
        public string BenefitId { get; set; }

        /// <summary>
        /// 蚂蚁会员权益配置在各个用户等级下的折扣积分
        /// </summary>
        [XmlArray("grade_points")]
        [XmlArrayItem("grade_discount_point")]
        public List<GradeDiscountPoint> GradePoints { get; set; }

        /// <summary>
        /// 蚂蚁会员权益配置的原始积分
        /// </summary>
        [XmlElement("original_point")]
        public string OriginalPoint { get; set; }

        /// <summary>
        /// 蚂蚁会员权益的专享等级列表
        /// </summary>
        [XmlElement("own_grades")]
        public string OwnGrades { get; set; }
    }
}

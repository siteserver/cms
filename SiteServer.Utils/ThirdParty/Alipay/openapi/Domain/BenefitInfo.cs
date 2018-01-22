using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// BenefitInfo Data Structure.
    /// </summary>
    [Serializable]
    public class BenefitInfo : AopObject
    {
        /// <summary>
        /// 权益信息id
        /// </summary>
        [XmlElement("benefit_info_id")]
        public string BenefitInfoId { get; set; }

        /// <summary>
        /// 权益名称
        /// </summary>
        [XmlElement("benefit_name")]
        public string BenefitName { get; set; }

        /// <summary>
        /// 权益中文名称
        /// </summary>
        [XmlElement("benefit_name_cn")]
        public string BenefitNameCn { get; set; }

        /// <summary>
        /// 权益类型(会员等级: MemberGrade)
        /// </summary>
        [XmlElement("benefit_type")]
        public string BenefitType { get; set; }

        /// <summary>
        /// 权益发放时间
        /// </summary>
        [XmlElement("dispatch_dt")]
        public string DispatchDt { get; set; }

        /// <summary>
        /// 权益失效时间
        /// </summary>
        [XmlElement("end_dt")]
        public string EndDt { get; set; }

        /// <summary>
        /// 权益生效时间
        /// </summary>
        [XmlElement("start_dt")]
        public string StartDt { get; set; }

        /// <summary>
        /// 权益当前状态       * 待生效：WAIT  * 生效：VALID  * 失效：INVALID
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }
    }
}

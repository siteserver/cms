using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// InsCoverage Data Structure.
    /// </summary>
    [Serializable]
    public class InsCoverage : AopObject
    {
        /// <summary>
        /// 险种名称
        /// </summary>
        [XmlElement("coverage_name")]
        public string CoverageName { get; set; }

        /// <summary>
        /// 险种编号
        /// </summary>
        [XmlElement("coverage_no")]
        public string CoverageNo { get; set; }

        /// <summary>
        /// 险种失效时间
        /// </summary>
        [XmlElement("effect_end_time")]
        public string EffectEndTime { get; set; }

        /// <summary>
        /// 险种生效时间
        /// </summary>
        [XmlElement("effect_start_time")]
        public string EffectStartTime { get; set; }

        /// <summary>
        /// 不计免赔;0：默认不投保;  1：默认投保。
        /// </summary>
        [XmlElement("iop")]
        public long Iop { get; set; }

        /// <summary>
        /// 不计免赔的保费
        /// </summary>
        [XmlElement("iop_premium")]
        public long IopPremium { get; set; }

        /// <summary>
        /// 保费;单位分
        /// </summary>
        [XmlElement("premium")]
        public long Premium { get; set; }

        /// <summary>
        /// 保额;单位分
        /// </summary>
        [XmlElement("sum_insured")]
        public long SumInsured { get; set; }
    }
}

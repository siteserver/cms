using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// InsSumInsured Data Structure.
    /// </summary>
    [Serializable]
    public class InsSumInsured : AopObject
    {
        /// <summary>
        /// 保额默认值;单位分
        /// </summary>
        [XmlElement("default_value")]
        public long DefaultValue { get; set; }

        /// <summary>
        /// 保额最大值;单位分,当sum_insured_type=MONEY_RANGE时该值有效
        /// </summary>
        [XmlElement("max_value")]
        public long MaxValue { get; set; }

        /// <summary>
        /// 保额最小值;单位分,当sum_insured_type=MONEY_RANGE时该值有效
        /// </summary>
        [XmlElement("min_value")]
        public long MinValue { get; set; }

        /// <summary>
        /// 保额类型;MONEY_RANGE:金额范围,MONEY_LIST:金额可选值,ENUM_VALUE:枚举值
        /// </summary>
        [XmlElement("sum_insured_type")]
        public string SumInsuredType { get; set; }

        /// <summary>
        /// 保额列表;列表里的值单位为分,当sum_insured_type=MONEY_LIST时该值有效
        /// </summary>
        [XmlArray("sum_insureds")]
        [XmlArrayItem("number")]
        public List<long> SumInsureds { get; set; }
    }
}

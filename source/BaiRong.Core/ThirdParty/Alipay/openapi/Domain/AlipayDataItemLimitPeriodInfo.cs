using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayDataItemLimitPeriodInfo Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayDataItemLimitPeriodInfo : AopObject
    {
        /// <summary>
        /// 区间范围枚举，分为： INCLUDE（包含） EXCLUDE（排除）
        /// </summary>
        [XmlElement("rule")]
        public string Rule { get; set; }

        /// <summary>
        /// 单位描述，分为：  MINUTE（分钟）  HOUR（小时）  WEEK_DAY（星期几）  DAY（日）  WEEK（周）  MONTH（月）  ALL（整个销售周期）
        /// </summary>
        [XmlElement("unit")]
        public string Unit { get; set; }

        /// <summary>
        /// 区间范围值
        /// </summary>
        [XmlArray("value")]
        [XmlArrayItem("number")]
        public List<long> Value { get; set; }
    }
}

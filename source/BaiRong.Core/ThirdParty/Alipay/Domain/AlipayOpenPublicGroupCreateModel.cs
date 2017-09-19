using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayOpenPublicGroupCreateModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayOpenPublicGroupCreateModel : AopObject
    {
        /// <summary>
        /// 标签规则，满足该规则的粉丝将被圈定，标签id不能重复
        /// </summary>
        [XmlArray("label_rule")]
        [XmlArrayItem("complex_label_rule")]
        public List<ComplexLabelRule> LabelRule { get; set; }

        /// <summary>
        /// 分组名称，仅支持中文、字母、数字、下划线的组合。
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }
    }
}

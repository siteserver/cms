using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayOpenPublicGroupCrowdQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayOpenPublicGroupCrowdQueryModel : AopObject
    {
        /// <summary>
        /// 用户分组的规则项列表，规则项之间元素是与的逻辑，每个规则项内部用多个值表示或的逻辑
        /// </summary>
        [XmlArray("label_rule")]
        [XmlArrayItem("complex_label_rule")]
        public List<ComplexLabelRule> LabelRule { get; set; }
    }
}

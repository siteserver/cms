using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// QueryMenu Data Structure.
    /// </summary>
    [Serializable]
    public class QueryMenu : AopObject
    {
        /// <summary>
        /// 一级菜单列表
        /// </summary>
        [XmlArray("button")]
        [XmlArrayItem("button_object")]
        public List<ButtonObject> Button { get; set; }

        /// <summary>
        /// 标签规则项列表
        /// </summary>
        [XmlArray("label_rule")]
        [XmlArrayItem("query_label_rule")]
        public List<QueryLabelRule> LabelRule { get; set; }

        /// <summary>
        /// 菜单唯一id
        /// </summary>
        [XmlElement("menu_key")]
        public string MenuKey { get; set; }

        /// <summary>
        /// 菜单类型，icon：icon型菜单，text：文本型菜单
        /// </summary>
        [XmlElement("type")]
        public string Type { get; set; }
    }
}

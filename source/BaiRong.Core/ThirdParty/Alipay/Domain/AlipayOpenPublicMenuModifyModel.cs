using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayOpenPublicMenuModifyModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayOpenPublicMenuModifyModel : AopObject
    {
        /// <summary>
        /// 一级菜单列表。最多有4个一级菜单，若开发者在后台打开了"咨询反馈"的开关，则只能有3个一级菜单.
        /// </summary>
        [XmlArray("button")]
        [XmlArrayItem("button_object")]
        public List<ButtonObject> Button { get; set; }

        /// <summary>
        /// 菜单类型，支持值为icon：icon型菜单，text：文本型菜单，不传时默认为"text"，当传值为"icon"时，菜单节点的icon字段必传。
        /// </summary>
        [XmlElement("type")]
        public string Type { get; set; }
    }
}

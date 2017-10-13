using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// PrizeCustomMenu Data Structure.
    /// </summary>
    [Serializable]
    public class PrizeCustomMenu : AopObject
    {
        /// <summary>
        /// 菜单详情
        /// </summary>
        [XmlElement("text")]
        public string Text { get; set; }

        /// <summary>
        /// 菜单名称
        /// </summary>
        [XmlElement("title")]
        public string Title { get; set; }
    }
}

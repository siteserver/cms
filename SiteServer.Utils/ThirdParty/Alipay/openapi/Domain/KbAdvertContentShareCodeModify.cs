using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KbAdvertContentShareCodeModify Data Structure.
    /// </summary>
    [Serializable]
    public class KbAdvertContentShareCodeModify : AopObject
    {
        /// <summary>
        /// 宣传展示标题（不能超过30个字符）
        /// </summary>
        [XmlElement("display_title")]
        public string DisplayTitle { get; set; }
    }
}

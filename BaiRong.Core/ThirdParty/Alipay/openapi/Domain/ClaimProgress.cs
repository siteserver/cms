using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ClaimProgress Data Structure.
    /// </summary>
    [Serializable]
    public class ClaimProgress : AopObject
    {
        /// <summary>
        /// 更新内容
        /// </summary>
        [XmlElement("update_content")]
        public string UpdateContent { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [XmlElement("update_time")]
        public string UpdateTime { get; set; }
    }
}

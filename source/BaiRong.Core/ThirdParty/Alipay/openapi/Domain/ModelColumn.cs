using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ModelColumn Data Structure.
    /// </summary>
    [Serializable]
    public class ModelColumn : AopObject
    {
        /// <summary>
        /// 列别名
        /// </summary>
        [XmlElement("alias")]
        public string Alias { get; set; }

        /// <summary>
        /// 列值
        /// </summary>
        [XmlElement("data")]
        public string Data { get; set; }
    }
}

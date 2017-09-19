using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// EduSourceInfo Data Structure.
    /// </summary>
    [Serializable]
    public class EduSourceInfo : AopObject
    {
        /// <summary>
        /// 供应商的LOGO
        /// </summary>
        [XmlElement("logo")]
        public string Logo { get; set; }

        /// <summary>
        /// 供应商电话
        /// </summary>
        [XmlElement("mobile")]
        public string Mobile { get; set; }

        /// <summary>
        /// 供应商名字
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }
    }
}

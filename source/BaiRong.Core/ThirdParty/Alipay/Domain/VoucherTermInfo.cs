using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// VoucherTermInfo Data Structure.
    /// </summary>
    [Serializable]
    public class VoucherTermInfo : AopObject
    {
        /// <summary>
        /// 描述信息
        /// </summary>
        [XmlArray("descriptions")]
        [XmlArrayItem("string")]
        public List<string> Descriptions { get; set; }

        /// <summary>
        /// 详情title
        /// </summary>
        [XmlElement("title")]
        public string Title { get; set; }
    }
}

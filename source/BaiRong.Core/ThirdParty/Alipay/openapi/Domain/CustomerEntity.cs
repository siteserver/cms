using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// CustomerEntity Data Structure.
    /// </summary>
    [Serializable]
    public class CustomerEntity : AopObject
    {
        /// <summary>
        /// 实体英文名
        /// </summary>
        [XmlElement("code")]
        public string Code { get; set; }

        /// <summary>
        /// 实体描述
        /// </summary>
        [XmlElement("desc")]
        public string Desc { get; set; }

        /// <summary>
        /// 实体id
        /// </summary>
        [XmlElement("id")]
        public string Id { get; set; }

        /// <summary>
        /// 实体中文名称
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// tag列表
        /// </summary>
        [XmlArray("tags")]
        [XmlArrayItem("customer_tag")]
        public List<CustomerTag> Tags { get; set; }
    }
}

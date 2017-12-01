using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// InsMerchant Data Structure.
    /// </summary>
    [Serializable]
    public class InsMerchant : AopObject
    {
        /// <summary>
        /// 机构全称
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// 服务电话
        /// </summary>
        [XmlElement("service_phone")]
        public string ServicePhone { get; set; }

        /// <summary>
        /// 机构简称
        /// </summary>
        [XmlElement("short_name")]
        public string ShortName { get; set; }
    }
}

using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// SkuPropertyValue Data Structure.
    /// </summary>
    [Serializable]
    public class SkuPropertyValue : AopObject
    {
        /// <summary>
        /// 外部sku属性值ID,由外部商户定义
        /// </summary>
        [XmlElement("out_pv_id")]
        public string OutPvId { get; set; }

        /// <summary>
        /// 外部商户sku属性值
        /// </summary>
        [XmlElement("value")]
        public string Value { get; set; }
    }
}

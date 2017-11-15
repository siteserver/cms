using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// InvoiceInfo Data Structure.
    /// </summary>
    [Serializable]
    public class InvoiceInfo : AopObject
    {
        /// <summary>
        /// 开票内容  注：json数组格式
        /// </summary>
        [XmlElement("details")]
        public string Details { get; set; }

        /// <summary>
        /// 开票关键信息
        /// </summary>
        [XmlElement("key_info")]
        public InvoiceKeyInfo KeyInfo { get; set; }
    }
}

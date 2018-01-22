using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayKeyanClass Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayKeyanClass : AopObject
    {
        /// <summary>
        /// 1
        /// </summary>
        [XmlElement("user_name")]
        public string UserName { get; set; }
    }
}

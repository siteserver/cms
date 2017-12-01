using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayOpenPublicLabelModifyModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayOpenPublicLabelModifyModel : AopObject
    {
        /// <summary>
        /// 要修改的标签id
        /// </summary>
        [XmlElement("id")]
        public string Id { get; set; }

        /// <summary>
        /// 要修改成的标签名
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }
    }
}

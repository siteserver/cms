using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayOpenWangyanTestDeleteModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayOpenWangyanTestDeleteModel : AopObject
    {
        /// <summary>
        /// 1
        /// </summary>
        [XmlElement("aaa")]
        public string Aaa { get; set; }

        /// <summary>
        /// 2
        /// </summary>
        [XmlElement("user_name")]
        public string UserName { get; set; }
    }
}

using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayOpenPublicGisQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayOpenPublicGisQueryModel : AopObject
    {
        /// <summary>
        /// 该用户的userId
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}

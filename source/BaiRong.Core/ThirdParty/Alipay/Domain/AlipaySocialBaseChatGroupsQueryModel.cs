using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipaySocialBaseChatGroupsQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipaySocialBaseChatGroupsQueryModel : AopObject
    {
        /// <summary>
        /// 上次接口返回的key，初始传0
        /// </summary>
        [XmlElement("last_key")]
        public long LastKey { get; set; }
    }
}

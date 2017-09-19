using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipaySocialBaseChatGmemberDeleteModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipaySocialBaseChatGmemberDeleteModel : AopObject
    {
        /// <summary>
        /// 群id
        /// </summary>
        [XmlElement("group_id")]
        public string GroupId { get; set; }

        /// <summary>
        /// 剔除的群成员用户id列表
        /// </summary>
        [XmlArray("uids")]
        [XmlArrayItem("string")]
        public List<string> Uids { get; set; }
    }
}

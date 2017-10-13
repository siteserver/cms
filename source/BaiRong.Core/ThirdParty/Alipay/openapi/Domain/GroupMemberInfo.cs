using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// GroupMemberInfo Data Structure.
    /// </summary>
    [Serializable]
    public class GroupMemberInfo : AopObject
    {
        /// <summary>
        /// 用户在这个群里的昵称
        /// </summary>
        [XmlElement("group_nickname")]
        public string GroupNickname { get; set; }

        /// <summary>
        /// 个人昵称
        /// </summary>
        [XmlElement("nickname")]
        public string Nickname { get; set; }

        /// <summary>
        /// 用户的uid
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}

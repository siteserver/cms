using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// MemberInfo Data Structure.
    /// </summary>
    [Serializable]
    public class MemberInfo : AopObject
    {
        /// <summary>
        /// 群成员性别，m表示男，f表示女
        /// </summary>
        [XmlElement("gender")]
        public string Gender { get; set; }

        /// <summary>
        /// 群内昵称
        /// </summary>
        [XmlElement("group_nick_name")]
        public string GroupNickName { get; set; }

        /// <summary>
        /// 邀请人id
        /// </summary>
        [XmlElement("inviter_uid")]
        public string InviterUid { get; set; }

        /// <summary>
        /// 群成员头像url
        /// </summary>
        [XmlElement("member_img")]
        public string MemberImg { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        [XmlElement("nick_name")]
        public string NickName { get; set; }

        /// <summary>
        /// 群成员用户id
        /// </summary>
        [XmlElement("uid")]
        public string Uid { get; set; }
    }
}

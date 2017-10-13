using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ContactFollower Data Structure.
    /// </summary>
    [Serializable]
    public class ContactFollower : AopObject
    {
        /// <summary>
        /// 支付宝头像
        /// </summary>
        [XmlElement("avatar")]
        public string Avatar { get; set; }

        /// <summary>
        /// 默认头像
        /// </summary>
        [XmlElement("default_avatar")]
        public string DefaultAvatar { get; set; }

        /// <summary>
        /// false
        /// </summary>
        [XmlElement("each_record_flag")]
        public string EachRecordFlag { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}

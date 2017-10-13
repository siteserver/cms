using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// GroupSetting Data Structure.
    /// </summary>
    [Serializable]
    public class GroupSetting : AopObject
    {
        /// <summary>
        /// 群名称
        /// </summary>
        [XmlElement("group_name")]
        public string GroupName { get; set; }

        /// <summary>
        /// 是否开放群成员邀请
        /// </summary>
        [XmlElement("is_openinv")]
        public bool IsOpeninv { get; set; }

        /// <summary>
        /// 群公告
        /// </summary>
        [XmlElement("public_notice")]
        public string PublicNotice { get; set; }
    }
}

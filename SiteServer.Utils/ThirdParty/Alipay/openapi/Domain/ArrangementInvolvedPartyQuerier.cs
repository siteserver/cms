using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ArrangementInvolvedPartyQuerier Data Structure.
    /// </summary>
    [Serializable]
    public class ArrangementInvolvedPartyQuerier : AopObject
    {
        /// <summary>
        /// 参与者id
        /// </summary>
        [XmlElement("ip_id")]
        public string IpId { get; set; }

        /// <summary>
        /// 用户uid/参与者角色id
        /// </summary>
        [XmlElement("ip_role_id")]
        public string IpRoleId { get; set; }

        /// <summary>
        /// 参与者角色类型，为空时表示所有类型都查询.  可选值：01 甲方 11 乙方 21丙方
        /// </summary>
        [XmlElement("ip_type")]
        public string IpType { get; set; }
    }
}

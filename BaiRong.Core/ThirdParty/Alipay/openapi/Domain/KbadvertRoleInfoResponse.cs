using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KbadvertRoleInfoResponse Data Structure.
    /// </summary>
    [Serializable]
    public class KbadvertRoleInfoResponse : AopObject
    {
        /// <summary>
        /// 角色code
        /// </summary>
        [XmlElement("role_code")]
        public string RoleCode { get; set; }

        /// <summary>
        /// NOT_OPEN:未开通  OPENED:已经开通
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }
    }
}

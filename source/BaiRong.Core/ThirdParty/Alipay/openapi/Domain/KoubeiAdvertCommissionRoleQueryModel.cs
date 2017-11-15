using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiAdvertCommissionRoleQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiAdvertCommissionRoleQueryModel : AopObject
    {
        /// <summary>
        /// 角色code码，如果不提供该字段，则会遍历所有角色，并返回user_identify是否拥有，  MISSION_PROMO：任务推广角色  MISSION_PUBLISH：任务发布角色
        /// </summary>
        [XmlElement("role_code")]
        public string RoleCode { get; set; }

        /// <summary>
        /// 用户身份主键  user_identify_type=user_id -值必须是支付宝账户ID
        /// </summary>
        [XmlElement("user_identify")]
        public string UserIdentify { get; set; }

        /// <summary>
        /// 用户身份主键类型  user_id - 支付宝账户ID
        /// </summary>
        [XmlElement("user_identify_type")]
        public string UserIdentifyType { get; set; }
    }
}

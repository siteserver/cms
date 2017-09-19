using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipaySecurityRiskMobileactivityQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipaySecurityRiskMobileactivityQueryModel : AopObject
    {
        /// <summary>
        /// 账户绑定手机号
        /// </summary>
        [XmlElement("mobile")]
        public string Mobile { get; set; }

        /// <summary>
        /// 场景名称
        /// </summary>
        [XmlElement("scene_id")]
        public string SceneId { get; set; }

        /// <summary>
        /// 支付宝userId
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}

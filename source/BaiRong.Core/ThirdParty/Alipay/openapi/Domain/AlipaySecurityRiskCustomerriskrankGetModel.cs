using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipaySecurityRiskCustomerriskrankGetModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipaySecurityRiskCustomerriskrankGetModel : AopObject
    {
        /// <summary>
        /// 证件号码，除了场景id必填，其他几个参数不能同时为空
        /// </summary>
        [XmlElement("card_no")]
        public string CardNo { get; set; }

        /// <summary>
        /// 证件类型，除了场景id必填，其他几个参数不能同时为空
        /// </summary>
        [XmlElement("card_type")]
        public string CardType { get; set; }

        /// <summary>
        /// 手机号，除了场景id必填，其他几个参数不能同时为空
        /// </summary>
        [XmlElement("mobile")]
        public string Mobile { get; set; }

        /// <summary>
        /// 场景id
        /// </summary>
        [XmlElement("scene_id")]
        public string SceneId { get; set; }

        /// <summary>
        /// 支付宝账户id ，除了场景id必填，其他几个参数不能同时为空
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}

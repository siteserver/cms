using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayMobileShakeUserQueryResponse.
    /// </summary>
    public class AlipayMobileShakeUserQueryResponse : AopResponse
    {
        /// <summary>
        /// 对应的业务信息
        /// </summary>
        [XmlElement("bizdata")]
        public string Bizdata { get; set; }

        /// <summary>
        /// 支付宝用户登录账户，可能是email或者手机号码
        /// </summary>
        [XmlElement("logon_id")]
        public string LogonId { get; set; }

        /// <summary>
        /// 对应的核销数据
        /// </summary>
        [XmlElement("pass_id")]
        public string PassId { get; set; }

        /// <summary>
        /// 支付宝用户ID
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}

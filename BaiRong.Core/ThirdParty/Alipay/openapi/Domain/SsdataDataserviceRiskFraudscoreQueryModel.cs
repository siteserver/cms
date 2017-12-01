using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// SsdataDataserviceRiskFraudscoreQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class SsdataDataserviceRiskFraudscoreQueryModel : AopObject
    {
        /// <summary>
        /// 商户和支付宝交互时，用于代表支付宝分配给商户ID
        /// </summary>
        [XmlElement("partner_id")]
        public string PartnerId { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        [XmlElement("sys_version")]
        public string SysVersion { get; set; }

        /// <summary>
        /// 蚂蚁统一会员ID
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}

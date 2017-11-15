using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// InfoCode Data Structure.
    /// </summary>
    [Serializable]
    public class InfoCode : AopObject
    {
        /// <summary>
        /// 风险描述
        /// </summary>
        [XmlElement("risk_description")]
        public string RiskDescription { get; set; }

        /// <summary>
        /// 风险因素编码
        /// </summary>
        [XmlElement("risk_factor_code")]
        public string RiskFactorCode { get; set; }

        /// <summary>
        /// 风险因素名称
        /// </summary>
        [XmlElement("risk_factor_name")]
        public string RiskFactorName { get; set; }

        /// <summary>
        /// 风险度量
        /// </summary>
        [XmlElement("risk_magnitude")]
        public string RiskMagnitude { get; set; }
    }
}

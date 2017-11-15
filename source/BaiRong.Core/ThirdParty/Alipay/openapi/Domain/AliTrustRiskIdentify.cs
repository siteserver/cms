using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AliTrustRiskIdentify Data Structure.
    /// </summary>
    [Serializable]
    public class AliTrustRiskIdentify : AopObject
    {
        /// <summary>
        /// 芝麻信用风险名单详情列表
        /// </summary>
        [XmlArray("details")]
        [XmlArrayItem("zhima_risk_detail")]
        public List<ZhimaRiskDetail> Details { get; set; }

        /// <summary>
        /// 当有风险时,为"T";无风险识别是为"F"
        /// </summary>
        [XmlElement("is_risk")]
        public string IsRisk { get; set; }

        /// <summary>
        /// 已废弃
        /// </summary>
        [XmlElement("risk_tag")]
        public string RiskTag { get; set; }
    }
}

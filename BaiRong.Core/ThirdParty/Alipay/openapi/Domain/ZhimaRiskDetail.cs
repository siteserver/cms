using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ZhimaRiskDetail Data Structure.
    /// </summary>
    [Serializable]
    public class ZhimaRiskDetail : AopObject
    {
        /// <summary>
        /// 数据类型：Negative(负面信息)、Risk(风险信息) 。系统会将在APP上对C端披露的信息标记为负面信息，其余的信息标记为风险信息。
        /// </summary>
        [XmlElement("data_type")]
        public string DataType { get; set; }

        /// <summary>
        /// 对于该条风险信息的补充信息。是名称和值得键值对。依据不同的风险类型，存在不同内容。返回信息为JSON字符串。
        /// </summary>
        [XmlElement("extendinfo")]
        public string Extendinfo { get; set; }

        /// <summary>
        /// 风险代码
        /// </summary>
        [XmlElement("risk_code")]
        public string RiskCode { get; set; }

        /// <summary>
        /// 风险类型
        /// </summary>
        [XmlElement("risk_type")]
        public string RiskType { get; set; }

        /// <summary>
        /// 对于逾期类风险信息，此字段表示是否结清。T（结清）/F（未结清）
        /// </summary>
        [XmlElement("settlement")]
        public string Settlement { get; set; }

        /// <summary>
        /// 当用户进行异议处理，并核查完毕之后，仍有异议时，给出声明。
        /// </summary>
        [XmlElement("statement")]
        public string Statement { get; set; }

        /// <summary>
        /// 当用户本人对该条负面记录有异议时，表示该异议处理流程的状态
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }

        /// <summary>
        /// 行业类型
        /// </summary>
        [XmlElement("type")]
        public string Type { get; set; }

        /// <summary>
        /// 数据更新时间
        /// </summary>
        [XmlElement("update")]
        public string Update { get; set; }
    }
}

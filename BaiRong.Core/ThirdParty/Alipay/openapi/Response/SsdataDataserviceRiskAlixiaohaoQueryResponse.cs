using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// SsdataDataserviceRiskAlixiaohaoQueryResponse.
    /// </summary>
    public class SsdataDataserviceRiskAlixiaohaoQueryResponse : AopResponse
    {
        /// <summary>
        /// 是否阿里小号
        /// </summary>
        [XmlElement("is_alixiaohao")]
        public bool IsAlixiaohao { get; set; }
    }
}

using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayMsaasMediarecogVoiceMediaaudioUploadResponse.
    /// </summary>
    public class AlipayMsaasMediarecogVoiceMediaaudioUploadResponse : AopResponse
    {
        /// <summary>
        /// 扩展字段
        /// </summary>
        [XmlElement("extinfo_a")]
        public string ExtinfoA { get; set; }

        /// <summary>
        /// 扩展字段
        /// </summary>
        [XmlElement("extinfo_b")]
        public string ExtinfoB { get; set; }

        /// <summary>
        /// 结果memo
        /// </summary>
        [XmlElement("result_memo")]
        public string ResultMemo { get; set; }

        /// <summary>
        /// 结果状态
        /// </summary>
        [XmlElement("result_status")]
        public string ResultStatus { get; set; }
    }
}

using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayMsaasMediarecogVoiceMediaaudioUploadModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayMsaasMediarecogVoiceMediaaudioUploadModel : AopObject
    {
        /// <summary>
        /// base64编码的声音数据
        /// </summary>
        [XmlElement("data")]
        public string Data { get; set; }

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
        /// 扩展字段
        /// </summary>
        [XmlElement("extinfo_c")]
        public string ExtinfoC { get; set; }

        /// <summary>
        /// 扩展字段
        /// </summary>
        [XmlElement("extinfo_d")]
        public string ExtinfoD { get; set; }

        /// <summary>
        /// 时间戳
        /// </summary>
        [XmlElement("labeltime")]
        public string Labeltime { get; set; }

        /// <summary>
        /// 媒体名称
        /// </summary>
        [XmlElement("vname")]
        public string Vname { get; set; }

        /// <summary>
        /// 媒体类型
        /// </summary>
        [XmlElement("vtype")]
        public string Vtype { get; set; }
    }
}

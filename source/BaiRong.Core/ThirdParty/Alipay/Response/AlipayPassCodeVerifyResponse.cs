using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayPassCodeVerifyResponse.
    /// </summary>
    public class AlipayPassCodeVerifyResponse : AopResponse
    {
        /// <summary>
        /// 返回核销流水号及操作结果
        /// </summary>
        [XmlElement("biz_result")]
        public string BizResult { get; set; }

        /// <summary>
        /// 返回码.
        /// </summary>
        [XmlElement("error_code")]
        public string ErrorCode { get; set; }

        /// <summary>
        /// 是否发码成功的标识。
        /// </summary>
        [XmlElement("success")]
        public bool Success { get; set; }
    }
}

using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayPassVerifyQueryResponse.
    /// </summary>
    public class AlipayPassVerifyQueryResponse : AopResponse
    {
        /// <summary>
        /// 查询成功时返回的结果信息
        /// </summary>
        [XmlElement("biz_result")]
        public string BizResult { get; set; }

        /// <summary>
        /// 返回码。
        /// </summary>
        [XmlElement("error_code")]
        public string ErrorCode { get; set; }

        /// <summary>
        /// 查询是否成功的标识
        /// </summary>
        [XmlElement("success")]
        public bool Success { get; set; }
    }
}

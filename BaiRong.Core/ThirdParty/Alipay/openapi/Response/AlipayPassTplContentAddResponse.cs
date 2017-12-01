using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayPassTplContentAddResponse.
    /// </summary>
    public class AlipayPassTplContentAddResponse : AopResponse
    {
        /// <summary>
        /// 业务结果
        /// </summary>
        [XmlElement("biz_result")]
        public string BizResult { get; set; }

        /// <summary>
        /// 业务错误码
        /// </summary>
        [XmlElement("error_code")]
        public string ErrorCode { get; set; }

        /// <summary>
        /// 是否成功标识:T-成功；F-失败
        /// </summary>
        [XmlElement("success")]
        public string Success { get; set; }
    }
}

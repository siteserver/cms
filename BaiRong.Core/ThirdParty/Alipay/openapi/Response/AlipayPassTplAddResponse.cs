using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayPassTplAddResponse.
    /// </summary>
    public class AlipayPassTplAddResponse : AopResponse
    {
        /// <summary>
        /// 接口返回业务错误码
        /// </summary>
        [XmlElement("error_code")]
        public string ErrorCode { get; set; }

        /// <summary>
        /// 接口调用返回结果信息
        /// </summary>
        [XmlElement("result")]
        public string Result { get; set; }

        /// <summary>
        /// 操作成功标识【T：成功；F：失败】
        /// </summary>
        [XmlElement("success")]
        public string Success { get; set; }
    }
}

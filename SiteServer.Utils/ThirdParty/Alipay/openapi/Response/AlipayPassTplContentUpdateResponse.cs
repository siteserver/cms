using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayPassTplContentUpdateResponse.
    /// </summary>
    public class AlipayPassTplContentUpdateResponse : AopResponse
    {
        /// <summary>
        /// 业务错误码
        /// </summary>
        [XmlElement("error_code")]
        public string ErrorCode { get; set; }

        /// <summary>
        /// 业务结果数据：包含serialNumber, pass_Id和操作类型
        /// </summary>
        [XmlElement("result")]
        public string Result { get; set; }

        /// <summary>
        /// 操作是否成功标识：T标识成功；F标识失败
        /// </summary>
        [XmlElement("success")]
        public string Success { get; set; }
    }
}

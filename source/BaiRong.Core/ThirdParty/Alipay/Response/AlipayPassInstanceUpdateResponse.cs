using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayPassInstanceUpdateResponse.
    /// </summary>
    public class AlipayPassInstanceUpdateResponse : AopResponse
    {
        /// <summary>
        /// 接口调用返回结果信息
        /// </summary>
        [XmlElement("result")]
        public string Result { get; set; }

        /// <summary>
        /// 操作成功标识【true：成功；false：失败】
        /// </summary>
        [XmlElement("success")]
        public string Success { get; set; }
    }
}

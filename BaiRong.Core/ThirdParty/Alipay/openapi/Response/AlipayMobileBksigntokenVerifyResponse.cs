using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayMobileBksigntokenVerifyResponse.
    /// </summary>
    public class AlipayMobileBksigntokenVerifyResponse : AopResponse
    {
        /// <summary>
        /// 返回值创建时间
        /// </summary>
        [XmlElement("createtimestamp")]
        public string Createtimestamp { get; set; }

        /// <summary>
        /// 返回值logonId
        /// </summary>
        [XmlElement("loginid")]
        public string Loginid { get; set; }

        /// <summary>
        /// 结果说明
        /// </summary>
        [XmlElement("memo")]
        public string Memo { get; set; }

        /// <summary>
        /// 结果状态码
        /// </summary>
        [XmlElement("resultcode")]
        public long Resultcode { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        [XmlElement("success")]
        public bool Success { get; set; }

        /// <summary>
        /// 返回值userId
        /// </summary>
        [XmlElement("userid")]
        public string Userid { get; set; }
    }
}

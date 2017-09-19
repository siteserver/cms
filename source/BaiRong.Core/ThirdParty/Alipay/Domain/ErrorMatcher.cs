using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ErrorMatcher Data Structure.
    /// </summary>
    [Serializable]
    public class ErrorMatcher : AopObject
    {
        /// <summary>
        /// 失败原因
        /// </summary>
        [XmlElement("error_msg")]
        public string ErrorMsg { get; set; }

        /// <summary>
        /// 用户打标接口出错的匹配器
        /// </summary>
        [XmlElement("matcher")]
        public Matcher Matcher { get; set; }
    }
}

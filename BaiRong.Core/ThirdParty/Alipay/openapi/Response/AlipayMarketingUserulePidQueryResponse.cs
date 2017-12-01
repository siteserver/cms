using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayMarketingUserulePidQueryResponse.
    /// </summary>
    public class AlipayMarketingUserulePidQueryResponse : AopResponse
    {
        /// <summary>
        /// 满足条件的所有pid，多个pid使用英文逗号隔开
        /// </summary>
        [XmlElement("pids")]
        public string Pids { get; set; }
    }
}

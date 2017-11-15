using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayPromorulecenterRuleAnalyzeResponse.
    /// </summary>
    public class AlipayPromorulecenterRuleAnalyzeResponse : AopResponse
    {
        /// <summary>
        /// 未通过条件描述信息
        /// </summary>
        [XmlElement("fail_condition_msg")]
        public string FailConditionMsg { get; set; }

        /// <summary>
        /// 未通过的条件
        /// </summary>
        [XmlElement("fail_condition_name")]
        public string FailConditionName { get; set; }

        /// <summary>
        /// 结果码
        /// </summary>
        [XmlElement("result_code")]
        public string ResultCode { get; set; }

        /// <summary>
        /// 服务调用是否成功
        /// </summary>
        [XmlElement("success")]
        public string Success { get; set; }

        /// <summary>
        /// 规则是否通过
        /// </summary>
        [XmlElement("triggered")]
        public string Triggered { get; set; }
    }
}

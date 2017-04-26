using System;
using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// AlibabaAliqinFcFlowQueryResponse.
    /// </summary>
    public class AlibabaAliqinFcFlowQueryResponse : TopResponse
    {
        /// <summary>
        /// 无
        /// </summary>
        [XmlElement("value")]
        public ResultDomain Value { get; set; }

	/// <summary>
/// ResultDomain Data Structure.
/// </summary>
[Serializable]
public class ResultDomain : TopObject
{
	        /// <summary>
	        /// 错误码
	        /// </summary>
	        [XmlElement("code")]
	        public string Code { get; set; }
	
	        /// <summary>
	        /// id：提交的唯一流水号, time:提交时间, phone:充值号码, error:false成功，true失败, reason:错误原因, status:1充值中，3成功，4失败, flow:流量, operator:运营商
	        /// </summary>
	        [XmlElement("model")]
	        public string Model { get; set; }
	
	        /// <summary>
	        /// 原因
	        /// </summary>
	        [XmlElement("msg")]
	        public string Msg { get; set; }
	
	        /// <summary>
	        /// 状态
	        /// </summary>
	        [XmlElement("success")]
	        public bool Success { get; set; }
}

    }
}

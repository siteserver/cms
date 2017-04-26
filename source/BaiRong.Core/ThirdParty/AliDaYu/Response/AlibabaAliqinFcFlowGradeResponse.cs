using System;
using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// AlibabaAliqinFcFlowGradeResponse.
    /// </summary>
    public class AlibabaAliqinFcFlowGradeResponse : TopResponse
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
	        /// 档位
	        /// </summary>
	        [XmlElement("model")]
	        public string Model { get; set; }
	
	        /// <summary>
	        /// 错误原因
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

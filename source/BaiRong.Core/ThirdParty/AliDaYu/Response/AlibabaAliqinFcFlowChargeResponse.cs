using System;
using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// AlibabaAliqinFcFlowChargeResponse.
    /// </summary>
    public class AlibabaAliqinFcFlowChargeResponse : TopResponse
    {
        /// <summary>
        /// 返回值
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
	        /// 无
	        /// </summary>
	        [XmlElement("model")]
	        public string Model { get; set; }
	
	        /// <summary>
	        /// 原因
	        /// </summary>
	        [XmlElement("msg")]
	        public string Msg { get; set; }
	
	        /// <summary>
	        /// true成功，false失败
	        /// </summary>
	        [XmlElement("success")]
	        public bool Success { get; set; }
}

    }
}

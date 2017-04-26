using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// AlibabaAliqinFcIotQrycardResponse.
    /// </summary>
    public class AlibabaAliqinFcIotQrycardResponse : TopResponse
    {
        /// <summary>
        /// 系统自动生成
        /// </summary>
        [XmlElement("result")]
        public ResultDomain Result { get; set; }

	/// <summary>
/// ModelDomain Data Structure.
/// </summary>
[Serializable]
public class ModelDomain : TopObject
{
	        /// <summary>
	        /// flowResource
	        /// </summary>
	        [XmlElement("flow_resource")]
	        public long FlowResource { get; set; }
	
	        /// <summary>
	        /// resourceType
	        /// </summary>
	        [XmlElement("resource_type")]
	        public string ResourceType { get; set; }
	
	        /// <summary>
	        /// restOfFlow
	        /// </summary>
	        [XmlElement("rest_of_flow")]
	        public long RestOfFlow { get; set; }
}

	/// <summary>
/// ResultDomain Data Structure.
/// </summary>
[Serializable]
public class ResultDomain : TopObject
{
	        /// <summary>
	        /// code
	        /// </summary>
	        [XmlElement("code")]
	        public string Code { get; set; }
	
	        /// <summary>
	        /// model
	        /// </summary>
	        [XmlArray("models")]
	        [XmlArrayItem("model")]
	        public List<ModelDomain> Models { get; set; }
	
	        /// <summary>
	        /// 系统自动生成
	        /// </summary>
	        [XmlElement("msg")]
	        public string Msg { get; set; }
	
	        /// <summary>
	        /// true返回成功，false返回失败
	        /// </summary>
	        [XmlElement("success")]
	        public bool Success { get; set; }
}

    }
}

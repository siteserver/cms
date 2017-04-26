using System;
using System.Xml.Serialization;

namespace Aliyun.Api
{
    [Serializable]
    public abstract class AliyunResponse
    {
        /// <summary>
        /// 错误码
        /// </summary>
        [XmlElement("Code")]
        public string Code { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        [XmlElement("Message")]
        public string Message { get; set; }
		

        /// <summary>
        /// 响应原始内容
        /// </summary>
        public string Body { get; set; }


        /// <summary>
        /// 响应结果是否错误
        /// </summary>
        public bool IsError => !string.IsNullOrEmpty(this.Code);
    }
}

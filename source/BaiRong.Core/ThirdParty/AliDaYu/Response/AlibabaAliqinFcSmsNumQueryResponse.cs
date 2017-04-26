using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// AlibabaAliqinFcSmsNumQueryResponse.
    /// </summary>
    public class AlibabaAliqinFcSmsNumQueryResponse : TopResponse
    {
        /// <summary>
        /// 当前页码
        /// </summary>
        [XmlElement("current_page")]
        public long CurrentPage { get; set; }

        /// <summary>
        /// 每页数量
        /// </summary>
        [XmlElement("page_size")]
        public long PageSize { get; set; }

        /// <summary>
        /// 总量
        /// </summary>
        [XmlElement("total_count")]
        public long TotalCount { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        [XmlElement("total_page")]
        public long TotalPage { get; set; }

        /// <summary>
        /// 1
        /// </summary>
        [XmlArray("values")]
        [XmlArrayItem("fc_partner_sms_detail_dto")]
        public List<FcPartnerSmsDetailDtoDomain> Values { get; set; }

	/// <summary>
/// FcPartnerSmsDetailDtoDomain Data Structure.
/// </summary>
[Serializable]
public class FcPartnerSmsDetailDtoDomain : TopObject
{
	        /// <summary>
	        /// 公共回传参数
	        /// </summary>
	        [XmlElement("extend")]
	        public string Extend { get; set; }
	
	        /// <summary>
	        /// 短信接收号码
	        /// </summary>
	        [XmlElement("rec_num")]
	        public string RecNum { get; set; }
	
	        /// <summary>
	        /// 短信错误码
	        /// </summary>
	        [XmlElement("result_code")]
	        public string ResultCode { get; set; }
	
	        /// <summary>
	        /// 模板编码
	        /// </summary>
	        [XmlElement("sms_code")]
	        public string SmsCode { get; set; }
	
	        /// <summary>
	        /// 短信发送内容
	        /// </summary>
	        [XmlElement("sms_content")]
	        public string SmsContent { get; set; }
	
	        /// <summary>
	        /// 短信接收时间
	        /// </summary>
	        [XmlElement("sms_receiver_time")]
	        public string SmsReceiverTime { get; set; }
	
	        /// <summary>
	        /// 短信发送时间
	        /// </summary>
	        [XmlElement("sms_send_time")]
	        public string SmsSendTime { get; set; }
	
	        /// <summary>
	        /// 发送状态 1：等待回执，2：发送失败，3：发送成功
	        /// </summary>
	        [XmlElement("sms_status")]
	        public long SmsStatus { get; set; }
}

    }
}

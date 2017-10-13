using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// CplifeNoticeDetail Data Structure.
    /// </summary>
    [Serializable]
    public class CplifeNoticeDetail : AopObject
    {
        /// <summary>
        /// 通告公告的具体内容
        /// </summary>
        [XmlElement("content")]
        public string Content { get; set; }

        /// <summary>
        /// 通知的下线时间.
        /// </summary>
        [XmlElement("end_time")]
        public string EndTime { get; set; }

        /// <summary>
        /// 通知公告中允许出现的图片列表。
        /// </summary>
        [XmlArray("image_list")]
        [XmlArrayItem("cplife_notice_img")]
        public List<CplifeNoticeImg> ImageList { get; set; }

        /// <summary>
        /// 通知的上线时间，该时刻之后，用户才能在支付宝客户端看到该通知。
        /// </summary>
        [XmlElement("start_time")]
        public string StartTime { get; set; }

        /// <summary>
        /// 通知是否置顶，默认为false.
        /// </summary>
        [XmlElement("stickied")]
        public bool Stickied { get; set; }

        /// <summary>
        /// 通知(公告)的标题
        /// </summary>
        [XmlElement("title")]
        public string Title { get; set; }

        /// <summary>
        /// 通告公告的具体类型.通知：“01” ,公告：“02”
        /// </summary>
        [XmlElement("type")]
        public string Type { get; set; }
    }
}

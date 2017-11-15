using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayOpenPublicContentPublishModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayOpenPublicContentPublishModel : AopObject
    {
        /// <summary>
        /// action_url 文章地址url，用于文章列表显示，用户点击后的跳转地址。
        /// </summary>
        [XmlElement("action_url")]
        public string ActionUrl { get; set; }

        /// <summary>
        /// article_id 为调用方的文章id，用于生活号对输入的文章进行去重检测
        /// </summary>
        [XmlElement("article_id")]
        public string ArticleId { get; set; }

        /// <summary>
        /// content 为写文章完整的正文文本内容
        /// </summary>
        [XmlElement("content")]
        public string Content { get; set; }

        /// <summary>
        /// cover_img 用于内容在文章列表中展示时的配图
        /// </summary>
        [XmlElement("cover_img")]
        public string CoverImg { get; set; }

        /// <summary>
        /// desc 用于描述文章简介
        /// </summary>
        [XmlElement("desc")]
        public string Desc { get; set; }

        /// <summary>
        /// endTime 用于描述文章内容有效截止时间
        /// </summary>
        [XmlElement("end_time")]
        public string EndTime { get; set; }

        /// <summary>
        /// source 用于描述调用接口的业务方
        /// </summary>
        [XmlElement("source")]
        public string Source { get; set; }

        /// <summary>
        /// title 用于描述文章标题
        /// </summary>
        [XmlElement("title")]
        public string Title { get; set; }
    }
}

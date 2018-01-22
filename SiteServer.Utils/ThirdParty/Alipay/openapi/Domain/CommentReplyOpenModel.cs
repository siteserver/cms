using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// CommentReplyOpenModel Data Structure.
    /// </summary>
    [Serializable]
    public class CommentReplyOpenModel : AopObject
    {
        /// <summary>
        /// 回复内容，最多500字，不区分中英文
        /// </summary>
        [XmlElement("content")]
        public string Content { get; set; }

        /// <summary>
        /// 发表回复的操作员id
        /// </summary>
        [XmlElement("operator_id")]
        public string OperatorId { get; set; }

        /// <summary>
        /// 回复发表时间
        /// </summary>
        [XmlElement("reply_publish_time")]
        public string ReplyPublishTime { get; set; }
    }
}

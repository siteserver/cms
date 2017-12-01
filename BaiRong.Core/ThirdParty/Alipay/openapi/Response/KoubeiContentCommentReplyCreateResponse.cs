using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// KoubeiContentCommentReplyCreateResponse.
    /// </summary>
    public class KoubeiContentCommentReplyCreateResponse : AopResponse
    {
        /// <summary>
        /// 口碑评价id，即入参中的comment_id，后续可用于回复内容的增删改查
        /// </summary>
        [XmlElement("comment_id")]
        public string CommentId { get; set; }
    }
}

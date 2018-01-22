using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiContentCommentReplyCreateModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiContentCommentReplyCreateModel : AopObject
    {
        /// <summary>
        /// 服务商、服务商员工、商户、商户员工等口碑角色操作时必填，对应为接口koubei.member.data.oauth.query（口碑业务授权令牌查询）中的auth_code，默认有效期24小时；isv自身角色操作的时候，无需传该参数
        /// </summary>
        [XmlElement("auth_code")]
        public string AuthCode { get; set; }

        /// <summary>
        /// 口碑评价id 可通过koubei.content.comment.data.batchquery接口查询
        /// </summary>
        [XmlElement("comment_id")]
        public string CommentId { get; set; }

        /// <summary>
        /// 评价回复内容，回复的内容不超过500字，不区分中英文
        /// </summary>
        [XmlElement("content")]
        public string Content { get; set; }
    }
}

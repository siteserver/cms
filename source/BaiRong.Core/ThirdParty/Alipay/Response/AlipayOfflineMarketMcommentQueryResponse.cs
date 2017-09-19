using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayOfflineMarketMcommentQueryResponse.
    /// </summary>
    public class AlipayOfflineMarketMcommentQueryResponse : AopResponse
    {
        /// <summary>
        /// 本次交易号对应的评价在支付宝的唯一标识.评价id  【注意】当仅评论信息状态为：已评论【comment_status='HASCOMMENT'】，才会返回该值
        /// </summary>
        [XmlElement("comment_id")]
        public string CommentId { get; set; }

        /// <summary>
        /// 注意：该字段描述评论状态。目前该字段区分2种类型状态  当comment_status='NOCOMMENT' 标识该笔交易未发生过评论。  当comment_status='HASCOMMENT' 标识该笔交易已经评论。评论状态正常  当comment_status='DELETED'标识该笔交易曾经评论过，当前时间点，该评论已经删除
        /// </summary>
        [XmlElement("comment_status")]
        public string CommentStatus { get; set; }

        /// <summary>
        /// 评价时间  【注意】当仅评论信息状态为：已评论【comment_status='HASCOMMENT'】，才会返回该值
        /// </summary>
        [XmlElement("comment_time")]
        public string CommentTime { get; set; }

        /// <summary>
        /// 评价星级  【注意】当仅评论信息状态为：已评论【comment_status='HASCOMMENT'】，才会返回该值
        /// </summary>
        [XmlElement("score")]
        public long Score { get; set; }
    }
}

using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// MerchantshopCommentStatistic Data Structure.
    /// </summary>
    [Serializable]
    public class MerchantshopCommentStatistic : AopObject
    {
        /// <summary>
        /// 评论总数
        /// </summary>
        [XmlElement("comment_count")]
        public long CommentCount { get; set; }

        /// <summary>
        /// 评分（平均分），两位小数
        /// </summary>
        [XmlElement("score")]
        public long Score { get; set; }
    }
}

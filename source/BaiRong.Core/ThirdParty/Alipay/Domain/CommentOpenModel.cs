using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// CommentOpenModel Data Structure.
    /// </summary>
    [Serializable]
    public class CommentOpenModel : AopObject
    {
        /// <summary>
        /// 口碑评价id
        /// </summary>
        [XmlElement("comment_id")]
        public string CommentId { get; set; }

        /// <summary>
        /// 评价发表时间
        /// </summary>
        [XmlElement("comment_publish_time")]
        public string CommentPublishTime { get; set; }

        /// <summary>
        /// 评价内容，不超过2000字，不区分中英文
        /// </summary>
        [XmlElement("content")]
        public string Content { get; set; }

        /// <summary>
        /// 评价关联的手艺人id
        /// </summary>
        [XmlElement("craftsman_id")]
        public string CraftsmanId { get; set; }

        /// <summary>
        /// 评价上传图片，一条评价最多9张图片
        /// </summary>
        [XmlArray("images")]
        [XmlArrayItem("string")]
        public List<string> Images { get; set; }

        /// <summary>
        /// 评价回复
        /// </summary>
        [XmlElement("reply")]
        public CommentReplyOpenModel Reply { get; set; }

        /// <summary>
        /// 评分，1-5分，1分最低，5分最高，均为整数
        /// </summary>
        [XmlElement("score")]
        public long Score { get; set; }

        /// <summary>
        /// 评价对应的门店id
        /// </summary>
        [XmlElement("shop_id")]
        public string ShopId { get; set; }
    }
}

using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// LifeRecommendArticles Data Structure.
    /// </summary>
    [Serializable]
    public class LifeRecommendArticles : AopObject
    {
        /// <summary>
        /// 文章id
        /// </summary>
        [XmlElement("article_id")]
        public string ArticleId { get; set; }

        /// <summary>
        /// 文章封面图片
        /// </summary>
        [XmlElement("article_image")]
        public string ArticleImage { get; set; }

        /// <summary>
        /// 支付宝钱包的文章详情地址
        /// </summary>
        [XmlElement("article_link")]
        public string ArticleLink { get; set; }

        /// <summary>
        /// 文章阅读数
        /// </summary>
        [XmlElement("article_read_cnt")]
        public string ArticleReadCnt { get; set; }

        /// <summary>
        /// 文章来源
        /// </summary>
        [XmlElement("article_source")]
        public string ArticleSource { get; set; }

        /// <summary>
        /// 文章的标题
        /// </summary>
        [XmlElement("article_title")]
        public string ArticleTitle { get; set; }
    }
}

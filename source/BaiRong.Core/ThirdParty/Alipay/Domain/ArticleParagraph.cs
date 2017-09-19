using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ArticleParagraph Data Structure.
    /// </summary>
    [Serializable]
    public class ArticleParagraph : AopObject
    {
        /// <summary>
        /// 图片列表
        /// </summary>
        [XmlArray("pictures")]
        [XmlArrayItem("article_picture")]
        public List<ArticlePicture> Pictures { get; set; }

        /// <summary>
        /// 文章正文描述
        /// </summary>
        [XmlElement("text")]
        public string Text { get; set; }
    }
}

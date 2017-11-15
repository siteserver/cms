using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayDataItemDescription Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayDataItemDescription : AopObject
    {
        /// <summary>
        /// 标题下的描述列表
        /// </summary>
        [XmlArray("details")]
        [XmlArrayItem("string")]
        public List<string> Details { get; set; }

        /// <summary>
        /// 明细图片列表
        /// </summary>
        [XmlArray("images")]
        [XmlArrayItem("string")]
        public List<string> Images { get; set; }

        /// <summary>
        /// 描述标题，不得超过15个中文字符
        /// </summary>
        [XmlElement("title")]
        public string Title { get; set; }

        /// <summary>
        /// 套餐使用说明链接，必须是https的地址链接
        /// </summary>
        [XmlElement("url")]
        public string Url { get; set; }
    }
}

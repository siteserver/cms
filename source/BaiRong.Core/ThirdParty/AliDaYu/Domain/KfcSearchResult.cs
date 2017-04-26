using System;
using System.Xml.Serialization;

namespace Top.Api.Domain
{
    /// <summary>
    /// KfcSearchResult Data Structure.
    /// </summary>
    [Serializable]
    public class KfcSearchResult : TopObject
    {
        /// <summary>
        /// 过滤后的文本：  当匹配到B等级的词时，文本中的关键词被替换为*号，content即为关键词替换后的文本；  其他情况，content始终为null
        /// </summary>
        [XmlElement("content")]
        public string Content { get; set; }

        /// <summary>
        /// 匹配到的关键词的等级，值为null，或为A、B、C、D。  当匹配不到关键词时，值为null，否则值为A、B、C、D中的一个。  A、B、C、D等级按严重程度从高至低排列。
        /// </summary>
        [XmlElement("level")]
        public string Level { get; set; }

        /// <summary>
        /// 是否匹配到关键词,匹配到则为true.
        /// </summary>
        [XmlElement("matched")]
        public bool Matched { get; set; }
    }
}

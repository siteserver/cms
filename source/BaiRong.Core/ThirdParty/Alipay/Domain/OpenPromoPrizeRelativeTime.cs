using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// OpenPromoPrizeRelativeTime Data Structure.
    /// </summary>
    [Serializable]
    public class OpenPromoPrizeRelativeTime : AopObject
    {
        /// <summary>
        /// 时间维度,       MI：表示 分       H：表示 时       D：表示 日       W：表示 周       M：表示 月
        /// </summary>
        [XmlElement("dimension")]
        public string Dimension { get; set; }

        /// <summary>
        /// 相对值
        /// </summary>
        [XmlElement("value")]
        public string Value { get; set; }
    }
}

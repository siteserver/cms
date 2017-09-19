using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// DataTag Data Structure.
    /// </summary>
    [Serializable]
    public class DataTag : AopObject
    {
        /// <summary>
        /// 聚合方式NONE,COUNT,COUNT_DISTINCT,DISTINCT,MIN,MAX,SUM
        /// </summary>
        [XmlElement("aggregate")]
        public string Aggregate { get; set; }

        /// <summary>
        /// 列别名
        /// </summary>
        [XmlElement("alias")]
        public string Alias { get; set; }

        /// <summary>
        /// 标签CODE
        /// </summary>
        [XmlElement("code")]
        public string Code { get; set; }
    }
}

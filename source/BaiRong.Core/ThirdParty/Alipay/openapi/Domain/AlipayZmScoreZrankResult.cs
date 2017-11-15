using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayZmScoreZrankResult Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayZmScoreZrankResult : AopObject
    {
        /// <summary>
        /// 芝麻分分段 Z0-Z7
        /// </summary>
        [XmlElement("zrank")]
        public string Zrank { get; set; }
    }
}

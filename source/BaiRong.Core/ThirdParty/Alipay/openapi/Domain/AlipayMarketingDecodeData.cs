using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayMarketingDecodeData Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayMarketingDecodeData : AopObject
    {
        /// <summary>
        /// 钱包二维码码值
        /// </summary>
        [XmlElement("code")]
        public string Code { get; set; }
    }
}

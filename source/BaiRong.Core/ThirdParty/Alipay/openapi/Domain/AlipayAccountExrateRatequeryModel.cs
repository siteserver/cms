using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayAccountExrateRatequeryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayAccountExrateRatequeryModel : AopObject
    {
        /// <summary>
        /// 需要查询汇率的货币对，如果为空则返回当前支持的所有货币对的汇率
        /// </summary>
        [XmlElement("currency_pair")]
        public string CurrencyPair { get; set; }
    }
}

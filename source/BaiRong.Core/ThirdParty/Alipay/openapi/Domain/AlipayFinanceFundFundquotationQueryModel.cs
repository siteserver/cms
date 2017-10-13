using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayFinanceFundFundquotationQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayFinanceFundFundquotationQueryModel : AopObject
    {
        /// <summary>
        /// 基金编号：基金产品编号
        /// </summary>
        [XmlElement("fund_code")]
        public string FundCode { get; set; }
    }
}

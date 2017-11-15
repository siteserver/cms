using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// CreditResult Data Structure.
    /// </summary>
    [Serializable]
    public class CreditResult : AopObject
    {
        /// <summary>
        /// 授信金额
        /// </summary>
        [XmlElement("credit_line")]
        public string CreditLine { get; set; }

        /// <summary>
        /// 授信编号
        /// </summary>
        [XmlElement("credit_no")]
        public string CreditNo { get; set; }

        /// <summary>
        /// 一笔授信规定的从开始到结束的周期，例如12个月，30天等。
        /// </summary>
        [XmlElement("credit_term")]
        public long CreditTerm { get; set; }

        /// <summary>
        /// 年、月、日
        /// </summary>
        [XmlElement("credit_term_unit")]
        public string CreditTermUnit { get; set; }

        /// <summary>
        /// 当贷款机构给客户一个可使用的授信时，只有在某一个日期之后客户才能使用，这个日期就是授信使用生效日期。
        /// </summary>
        [XmlElement("effective_date")]
        public string EffectiveDate { get; set; }

        /// <summary>
        /// 当贷款机构给客户一个可使用的授信时，客户必须要在某一个日期之前必须支用，否则此笔授信就会失效。
        /// </summary>
        [XmlElement("expire_date")]
        public string ExpireDate { get; set; }

        /// <summary>
        /// 技术服务费
        /// </summary>
        [XmlElement("fee_rate")]
        public string FeeRate { get; set; }

        /// <summary>
        /// 贷款利率
        /// </summary>
        [XmlElement("interest_rate")]
        public string InterestRate { get; set; }

        /// <summary>
        /// 一笔授信支用规定的从开始到结束的周期，例如12个月，30天等。
        /// </summary>
        [XmlElement("loan_term")]
        public long LoanTerm { get; set; }

        /// <summary>
        /// 年、月、日
        /// </summary>
        [XmlElement("loan_term_unit")]
        public string LoanTermUnit { get; set; }

        /// <summary>
        /// 还款方式，例如等额本息
        /// </summary>
        [XmlElement("repayment_mode")]
        public string RepaymentMode { get; set; }
    }
}

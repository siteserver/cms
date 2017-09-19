using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// MybankCreditLoantradeLoanarRepayModel Data Structure.
    /// </summary>
    [Serializable]
    public class MybankCreditLoantradeLoanarRepayModel : AopObject
    {
        /// <summary>
        /// 贷款客户在网商的会员ID
        /// </summary>
        [XmlElement("cust_iprole_id")]
        public string CustIproleId { get; set; }

        /// <summary>
        /// 还款日，精确到日，格式为yyyyMMdd，必须是当天
        /// </summary>
        [XmlElement("date")]
        public string Date { get; set; }

        /// <summary>
        /// 贷款合约号
        /// </summary>
        [XmlElement("loan_ar_no")]
        public string LoanArNo { get; set; }

        /// <summary>
        /// 还款本金金额，单位默认为元，支持小数点两位，为了便于传输用合作方将数值型转换为字符串型
        /// </summary>
        [XmlElement("prin_amt")]
        public string PrinAmt { get; set; }

        /// <summary>
        /// 外部流水号格式：日期(8位)+序列号(8位）,序列号是数字，如00000001（必须是16位且符合该格式）
        /// </summary>
        [XmlElement("request_id")]
        public string RequestId { get; set; }
    }
}

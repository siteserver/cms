using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ApproveCreditResult Data Structure.
    /// </summary>
    [Serializable]
    public class ApproveCreditResult : AopObject
    {
        /// <summary>
        /// 费用列表，每个费用对象代码一个收费项； 若费用列表为空或空集合，表示不存在费用定价
        /// </summary>
        [XmlArray("charge_info_list")]
        [XmlArrayItem("loan_charge_info")]
        public List<LoanChargeInfo> ChargeInfoList { get; set; }

        /// <summary>
        /// 授信金额
        /// </summary>
        [XmlElement("credit_amt")]
        public string CreditAmt { get; set; }

        /// <summary>
        /// 授信编号
        /// </summary>
        [XmlElement("credit_no")]
        public string CreditNo { get; set; }

        /// <summary>
        /// 授信期限长度，包含单位(年、月、日)
        /// </summary>
        [XmlElement("credit_term")]
        public string CreditTerm { get; set; }

        /// <summary>
        /// 授信有效截止日期(日期精度为天,包含截止日)
        /// </summary>
        [XmlElement("expire_date")]
        public string ExpireDate { get; set; }

        /// <summary>
        /// 利率
        /// </summary>
        [XmlArray("instal_int_rate")]
        [XmlArrayItem("installment_meta_info")]
        public List<InstallmentMetaInfo> InstalIntRate { get; set; }

        /// <summary>
        /// 贷款期限长度，包含单位(年、月、日)
        /// </summary>
        [XmlElement("loan_term")]
        public string LoanTerm { get; set; }

        /// <summary>
        /// 还款方式。若为分段还款，则存储的为分段还款方式的分段值。否则，该list仅含一个元素，为当前的还款方式
        /// </summary>
        [XmlArray("repay_modes")]
        [XmlArrayItem("installment_meta_info")]
        public List<InstallmentMetaInfo> RepayModes { get; set; }

        /// <summary>
        /// 授信有效起始日期(日期精度为天,包含起始日)
        /// </summary>
        [XmlElement("start_date")]
        public string StartDate { get; set; }

        /// <summary>
        /// 授信状态
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }
    }
}

using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// MybankCreditLoantradeRepayBudgetQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class MybankCreditLoantradeRepayBudgetQueryModel : AopObject
    {
        /// <summary>
        /// 申请还款本金
        /// </summary>
        [XmlElement("apply_repay_prin")]
        public string ApplyRepayPrin { get; set; }

        /// <summary>
        /// 扩展字段
        /// </summary>
        [XmlElement("ext_data")]
        public string ExtData { get; set; }

        /// <summary>
        /// 银行参与者id，是在网商银行创建会员后生成的id，网商银行会员的唯一标识
        /// </summary>
        [XmlElement("ip_id")]
        public string IpId { get; set; }

        /// <summary>
        /// 银行参与者角色id，是在网商银行创建会员后生成的角色id，网商银行会员角色的唯一标识
        /// </summary>
        [XmlElement("ip_role_id")]
        public string IpRoleId { get; set; }

        /// <summary>
        /// 贷款合约编号
        /// </summary>
        [XmlElement("loan_ar_no")]
        public string LoanArNo { get; set; }
    }
}

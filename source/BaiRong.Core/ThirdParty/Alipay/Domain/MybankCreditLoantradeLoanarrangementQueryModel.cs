using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// MybankCreditLoantradeLoanarrangementQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class MybankCreditLoantradeLoanarrangementQueryModel : AopObject
    {
        /// <summary>
        /// 网商银行参与者会员角色ID。客户在网商融资平台页面发起贷款申请或者机构调用代客户申贷接口mybank.credit.loanapply.apply.create后，网商会把申请结果以消息的方式通知机构，该字段包含在返回的消息体中。
        /// </summary>
        [XmlElement("ip_role_id")]
        public string IpRoleId { get; set; }

        /// <summary>
        /// 合约编号，客户签署合约时获取。
        /// </summary>
        [XmlElement("loan_ar_no")]
        public string LoanArNo { get; set; }
    }
}

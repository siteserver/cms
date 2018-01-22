using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// MybankCreditLoantradePartnerPaymentQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class MybankCreditLoantradePartnerPaymentQueryModel : AopObject
    {
        /// <summary>
        /// 网商内部申请单编号，外部机构根据此编号查询申请状态。
        /// </summary>
        [XmlElement("in_apply_no")]
        public string InApplyNo { get; set; }
    }
}

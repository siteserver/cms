using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// PromoteDataModel Data Structure.
    /// </summary>
    [Serializable]
    public class PromoteDataModel : AopObject
    {
        /// <summary>
        /// 核销数
        /// </summary>
        [XmlElement("checked_voucher_num")]
        public long CheckedVoucherNum { get; set; }

        /// <summary>
        /// 领券数
        /// </summary>
        [XmlElement("claim_voucher_num")]
        public long ClaimVoucherNum { get; set; }

        /// <summary>
        /// 分佣金额
        /// </summary>
        [XmlElement("commission_amount")]
        public string CommissionAmount { get; set; }
    }
}

using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// CodeCouponInfo Data Structure.
    /// </summary>
    [Serializable]
    public class CodeCouponInfo : AopObject
    {
        /// <summary>
        /// 领取时间
        /// </summary>
        [XmlElement("taken_time")]
        public string TakenTime { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [XmlElement("user_name")]
        public string UserName { get; set; }

        /// <summary>
        /// 面额（单位分）
        /// </summary>
        [XmlElement("voucher_amt")]
        public string VoucherAmt { get; set; }
    }
}

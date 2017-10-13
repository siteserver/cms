using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayUserCharityForestSendModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayUserCharityForestSendModel : AopObject
    {
        /// <summary>
        /// 唯一单据号，用于发能量幂等控制
        /// </summary>
        [XmlElement("biz_no")]
        public string BizNo { get; set; }

        /// <summary>
        /// 业务发生时间
        /// </summary>
        [XmlElement("biz_time")]
        public string BizTime { get; set; }

        /// <summary>
        /// 能量值，最小1g，最大100kg（100,000），不能有小数
        /// </summary>
        [XmlElement("energy")]
        public long Energy { get; set; }

        /// <summary>
        /// 能量气泡类型
        /// </summary>
        [XmlElement("energy_type")]
        public string EnergyType { get; set; }

        /// <summary>
        /// 业务来源
        /// </summary>
        [XmlElement("source")]
        public string Source { get; set; }

        /// <summary>
        /// 用户的支付宝账户ID
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}

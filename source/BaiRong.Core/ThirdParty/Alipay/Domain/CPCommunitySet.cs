using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// CPCommunitySet Data Structure.
    /// </summary>
    [Serializable]
    public class CPCommunitySet : AopObject
    {
        /// <summary>
        /// 支付宝社区小区统一编号。
        /// </summary>
        [XmlElement("community_id")]
        public string CommunityId { get; set; }

        /// <summary>
        /// 小区对应的物业公司支付宝账号PID（合作伙伴partner id）。物业公司给开发者做第三方应用授权后，开发者可获取物业公司PID。
        /// </summary>
        [XmlElement("merchant_pid")]
        public string MerchantPid { get; set; }

        /// <summary>
        /// 小区在物业系统中的唯一编号。若开发者传入过，则返回。
        /// </summary>
        [XmlElement("out_community_id")]
        public string OutCommunityId { get; set; }

        /// <summary>
        /// 小区当前状态,状态值：  PENDING_ONLINE 待上线  ONLINE - 上线  MAINTAIN - 维护中  OFFLINE - 下线
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }
    }
}

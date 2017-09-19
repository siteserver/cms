using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEcoCplifeCommunityDetailsQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEcoCplifeCommunityDetailsQueryModel : AopObject
    {
        /// <summary>
        /// 支付宝社区小区统一编号，必须在物业账号名下存在。
        /// </summary>
        [XmlElement("community_id")]
        public string CommunityId { get; set; }
    }
}

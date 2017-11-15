using System;
using System.Xml.Serialization;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayInsSceneCouponSendResponse.
    /// </summary>
    public class AlipayInsSceneCouponSendResponse : AopResponse
    {
        /// <summary>
        /// 发奖凭证
        /// </summary>
        [XmlElement("certificate")]
        public InsCertificate Certificate { get; set; }

        /// <summary>
        /// 活动ID
        /// </summary>
        [XmlElement("compaign_id")]
        public string CompaignId { get; set; }

        /// <summary>
        /// 发奖流水ID
        /// </summary>
        [XmlElement("flow_id")]
        public string FlowId { get; set; }

        /// <summary>
        /// 保险产品
        /// </summary>
        [XmlElement("product")]
        public InsProduct Product { get; set; }
    }
}

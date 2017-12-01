using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayMarketingCampaignDrawcampWhitelistCreateResponse.
    /// </summary>
    public class AlipayMarketingCampaignDrawcampWhitelistCreateResponse : AopResponse
    {
        /// <summary>
        /// 操作结果状态，业务维度，网关返回成功状态码并且业务维度返回true才成功
        /// </summary>
        [XmlElement("camp_result")]
        public bool CampResult { get; set; }
    }
}

using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayMarketingCampaignDrawcampUpdateResponse.
    /// </summary>
    public class AlipayMarketingCampaignDrawcampUpdateResponse : AopResponse
    {
        /// <summary>
        /// 操作结果状态，true表示修改成功立即生效，false表示修改失败
        /// </summary>
        [XmlElement("camp_result")]
        public bool CampResult { get; set; }
    }
}

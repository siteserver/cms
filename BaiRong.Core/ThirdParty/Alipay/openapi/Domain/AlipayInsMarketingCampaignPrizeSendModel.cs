using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayInsMarketingCampaignPrizeSendModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayInsMarketingCampaignPrizeSendModel : AopObject
    {
        /// <summary>
        /// 账户id，如支付宝userId：2088102161835009
        /// </summary>
        [XmlElement("account_id")]
        public string AccountId { get; set; }

        /// <summary>
        /// 账号类型
        /// </summary>
        [XmlElement("account_type")]
        public long AccountType { get; set; }

        /// <summary>
        /// 营销保险活动Id
        /// </summary>
        [XmlElement("campaign_id")]
        public string CampaignId { get; set; }

        /// <summary>
        /// 发奖规则因子
        /// </summary>
        [XmlArray("factors")]
        [XmlArrayItem("ins_mkt_factor_d_t_o")]
        public List<InsMktFactorDTO> Factors { get; set; }

        /// <summary>
        /// 发奖接口幂等字段
        /// </summary>
        [XmlElement("out_biz_no")]
        public string OutBizNo { get; set; }

        /// <summary>
        /// 请求流水Id
        /// </summary>
        [XmlElement("request_id")]
        public string RequestId { get; set; }
    }
}

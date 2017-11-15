using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// SsdataDataserviceRiskAntifraudscoreQueryResponse.
    /// </summary>
    public class SsdataDataserviceRiskAntifraudscoreQueryResponse : AopResponse
    {
        /// <summary>
        /// 蚁盾对于每一次请求返回的业务号。后续可以通过此业务号进行对账
        /// </summary>
        [XmlElement("biz_no")]
        public string BizNo { get; set; }

        /// <summary>
        /// 申请欺诈评分，分数范围是[0,100]的整数,分数越高信息越真实
        /// </summary>
        [XmlElement("score")]
        public string Score { get; set; }

        /// <summary>
        /// 用户唯一请求id
        /// </summary>
        [XmlElement("unique_id")]
        public string UniqueId { get; set; }
    }
}

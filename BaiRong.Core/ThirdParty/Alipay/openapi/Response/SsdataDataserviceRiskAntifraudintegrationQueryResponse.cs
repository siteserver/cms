using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// SsdataDataserviceRiskAntifraudintegrationQueryResponse.
    /// </summary>
    public class SsdataDataserviceRiskAntifraudintegrationQueryResponse : AopResponse
    {
        /// <summary>
        /// 蚁盾对于每一次请求返回的业务号。后续可以通过此业务号进行对账
        /// </summary>
        [XmlElement("biz_no")]
        public string BizNo { get; set; }

        /// <summary>
        /// 欺诈关注清单是否命中，yes标识命中，no标识未命中
        /// </summary>
        [XmlElement("hit")]
        public string Hit { get; set; }

        /// <summary>
        /// 欺诈关注清单的RiskCode列表，对应的描述见产品文档
        /// </summary>
        [XmlElement("risk_code")]
        public string RiskCode { get; set; }

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

        /// <summary>
        /// <a href="https://doc.open.alipay.com/docs/doc.htm?treeId=272&articleId=105917&docType=1">验证code列表</a>
        /// </summary>
        [XmlElement("verify_code")]
        public string VerifyCode { get; set; }
    }
}

using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipaySecurityRiskRainscoreQueryResponse.
    /// </summary>
    public class AlipaySecurityRiskRainscoreQueryResponse : AopResponse
    {
        /// <summary>
        /// 风险解释，即本次风险评分中TOP 3风险因子的代码、名称、解释、风险倍数（JSON格式）。详情请参考<a href="https://doc.open.alipay.com/doc2/detail.htm?treeId=214&articleId=104588&docType=1">《风险解释与身份标签》</a>
        /// </summary>
        [XmlArray("infocode")]
        [XmlArrayItem("info_code")]
        public List<InfoCode> Infocode { get; set; }

        /// <summary>
        /// 身份标签，即本次风险评分中评分主体（手机号）相关自然人的推测身份，例如：Scalper_3C（3C行业黄牛）等。没有与当前风险类型相关的推测身份时，身份标签可能为空。详情及申请方式请参考<a href="https://doc.open.alipay.com/doc2/detail.htm?treeId=214&articleId=104588&docType=1#s1">《风险解释及身份标签》</a>
        /// </summary>
        [XmlArray("label")]
        [XmlArrayItem("string")]
        public List<string> Label { get; set; }

        /// <summary>
        /// 风险评分，范围为[0,100]，评分越高风险越大
        /// </summary>
        [XmlElement("score")]
        public string Score { get; set; }

        /// <summary>
        /// 调用订单号
        /// </summary>
        [XmlElement("unique_id")]
        public string UniqueId { get; set; }
    }
}

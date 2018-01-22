using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayInsSceneProductAccessApplyModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayInsSceneProductAccessApplyModel : AopObject
    {
        /// <summary>
        /// 投保人
        /// </summary>
        [XmlElement("applicant")]
        public InsPerson Applicant { get; set; }

        /// <summary>
        /// 外部业务字段，幂等字段
        /// </summary>
        [XmlElement("out_biz_no")]
        public string OutBizNo { get; set; }

        /// <summary>
        /// 保险产品码
        /// </summary>
        [XmlElement("prod_code")]
        public string ProdCode { get; set; }

        /// <summary>
        /// 业务场景码
        /// </summary>
        [XmlElement("source")]
        public string Source { get; set; }
    }
}

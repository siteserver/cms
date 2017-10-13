using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayInsSceneApplicationBatchqueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayInsSceneApplicationBatchqueryModel : AopObject
    {
        /// <summary>
        /// 投保人
        /// </summary>
        [XmlElement("applicant")]
        public InsPerson Applicant { get; set; }

        /// <summary>
        /// 商户生成的外部投保业务号
        /// </summary>
        [XmlElement("out_biz_no")]
        public string OutBizNo { get; set; }

        /// <summary>
        /// 渠道来源
        /// </summary>
        [XmlElement("source")]
        public string Source { get; set; }
    }
}

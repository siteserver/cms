using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayMarketingFacetofaceTwostageUseModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayMarketingFacetofaceTwostageUseModel : AopObject
    {
        /// <summary>
        /// 业务场景码，外部商户在接入时需要进行分配
        /// </summary>
        [XmlElement("biz_sence")]
        public string BizSence { get; set; }

        /// <summary>
        /// 付钱码码值
        /// </summary>
        [XmlElement("dynamic_id")]
        public string DynamicId { get; set; }

        /// <summary>
        /// 业务扩展参数
        /// </summary>
        [XmlElement("ext_data")]
        public string ExtData { get; set; }

        /// <summary>
        /// 业务场景唯一编号，用于标识这笔请求，每次调用请勿使用相同的sence_no，每笔请求的sence_no必须不一样，支付时传递的DYNAMIC_TOKEN_OUT_BIZ_NO必须与调用开放平台传递的sence_no保持一致
        /// </summary>
        [XmlElement("sence_no")]
        public string SenceNo { get; set; }
    }
}

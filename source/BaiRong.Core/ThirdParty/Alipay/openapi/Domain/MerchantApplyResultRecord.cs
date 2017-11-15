using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// MerchantApplyResultRecord Data Structure.
    /// </summary>
    [Serializable]
    public class MerchantApplyResultRecord : AopObject
    {
        /// <summary>
        /// 签约产品名称
        /// </summary>
        [XmlElement("prod_name")]
        public string ProdName { get; set; }

        /// <summary>
        /// 如审核失败，表示该字段引发的审核失败
        /// </summary>
        [XmlElement("prop_input_key")]
        public string PropInputKey { get; set; }

        /// <summary>
        /// 认证、产品签约审核失败原因
        /// </summary>
        [XmlElement("result_msg")]
        public string ResultMsg { get; set; }

        /// <summary>
        /// 认证审核、产品签约审核状态
        /// </summary>
        [XmlElement("result_status")]
        public string ResultStatus { get; set; }

        /// <summary>
        /// 详情类型，CETIFY为认证，PROD为产品签约，COMMENT为认证审核批注
        /// </summary>
        [XmlElement("result_type")]
        public string ResultType { get; set; }
    }
}

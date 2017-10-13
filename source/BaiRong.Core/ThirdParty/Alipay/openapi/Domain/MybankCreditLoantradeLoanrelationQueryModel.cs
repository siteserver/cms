using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// MybankCreditLoantradeLoanrelationQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class MybankCreditLoantradeLoanrelationQueryModel : AopObject
    {
        /// <summary>
        /// 工商注册号或者身份证号码
        /// </summary>
        [XmlElement("cert_no")]
        public string CertNo { get; set; }

        /// <summary>
        /// 当客户为公司时，certtype是全国组织机构代码证书。当客户为个人时，是居民身份证
        /// </summary>
        [XmlElement("cert_type")]
        public string CertType { get; set; }

        /// <summary>
        /// 预留的扩展字段
        /// </summary>
        [XmlElement("ext_params")]
        public string ExtParams { get; set; }

        /// <summary>
        /// 政策码
        /// </summary>
        [XmlElement("loan_policy_code")]
        public string LoanPolicyCode { get; set; }

        /// <summary>
        /// 当客户是公司时，entityname是公司名全称；当客户是个人时，entityname是姓名
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// 产品码
        /// </summary>
        [XmlElement("product_code")]
        public string ProductCode { get; set; }
    }
}

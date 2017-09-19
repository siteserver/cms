using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayInsSceneCouponReceiveModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayInsSceneCouponReceiveModel : AopObject
    {
        /// <summary>
        /// 投保人
        /// </summary>
        [XmlElement("applicant")]
        public InsPerson Applicant { get; set; }

        /// <summary>
        /// 保险发奖凭证
        /// </summary>
        [XmlElement("certificate")]
        public InsCertificate Certificate { get; set; }

        /// <summary>
        /// 被保险人
        /// </summary>
        [XmlElement("insured")]
        public InsPerson Insured { get; set; }

        /// <summary>
        /// 市场类型;TAOBAO:淘宝平台,ANT: 蚂蚁平台
        /// </summary>
        [XmlElement("market_type")]
        public string MarketType { get; set; }

        /// <summary>
        /// 商户生成的外部业务号,必须保证唯一，幂等控制
        /// </summary>
        [XmlElement("out_biz_no")]
        public string OutBizNo { get; set; }

        /// <summary>
        /// 商户pid
        /// </summary>
        [XmlElement("partner_id")]
        public string PartnerId { get; set; }

        /// <summary>
        /// 产品编码;由蚂蚁保险平台分配
        /// </summary>
        [XmlElement("prod_code")]
        public string ProdCode { get; set; }

        /// <summary>
        /// 产品版本号
        /// </summary>
        [XmlElement("prod_version")]
        public string ProdVersion { get; set; }

        /// <summary>
        /// 服务场景;  propertyPaySuccess:蚂蚁物业支付成功页面
        /// </summary>
        [XmlElement("service_scenario")]
        public string ServiceScenario { get; set; }
    }
}

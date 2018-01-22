using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// InsPolicy Data Structure.
    /// </summary>
    [Serializable]
    public class InsPolicy : AopObject
    {
        /// <summary>
        /// 保单邮寄地址
        /// </summary>
        [XmlElement("addressee")]
        public InsAddressee Addressee { get; set; }

        /// <summary>
        /// 投保人
        /// </summary>
        [XmlElement("applicant")]
        public InsPerson Applicant { get; set; }

        /// <summary>
        /// 投保参数;标准json 格式
        /// </summary>
        [XmlElement("biz_data")]
        public string BizData { get; set; }

        /// <summary>
        /// 赔案
        /// </summary>
        [XmlArray("claims")]
        [XmlArrayItem("ins_claim")]
        public List<InsClaim> Claims { get; set; }

        /// <summary>
        /// 险种列表
        /// </summary>
        [XmlArray("coverages")]
        [XmlArrayItem("ins_coverage")]
        public List<InsCoverage> Coverages { get; set; }

        /// <summary>
        /// 保单失效时间
        /// </summary>
        [XmlElement("effect_end_time")]
        public string EffectEndTime { get; set; }

        /// <summary>
        /// 保单生效时间
        /// </summary>
        [XmlElement("effect_start_time")]
        public string EffectStartTime { get; set; }

        /// <summary>
        /// 标的列表
        /// </summary>
        [XmlArray("ins_objects")]
        [XmlArrayItem("ins_object")]
        public List<InsObject> InsObjects { get; set; }

        /// <summary>
        /// 被保险人
        /// </summary>
        [XmlArray("insureds")]
        [XmlArrayItem("ins_person")]
        public List<InsPerson> Insureds { get; set; }

        /// <summary>
        /// 机构名称
        /// </summary>
        [XmlElement("merchant_name")]
        public string MerchantName { get; set; }

        /// <summary>
        /// 保单凭证号;蚂蚁保险平台生成的保单凭证号,用户可以通过此单号去保险公司查询保单信息.
        /// </summary>
        [XmlElement("policy_no")]
        public string PolicyNo { get; set; }

        /// <summary>
        /// 保单状态.INEFFECTIVE:未生效、GUARANTEE:保障中、EXPIRED:已失效、SURRENDER:已退保
        /// </summary>
        [XmlElement("policy_status")]
        public string PolicyStatus { get; set; }

        /// <summary>
        /// 保费 ;单位分
        /// </summary>
        [XmlElement("premium")]
        public long Premium { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        [XmlElement("prod_name")]
        public string ProdName { get; set; }

        /// <summary>
        /// 保额 ;单位分
        /// </summary>
        [XmlElement("sum_insured")]
        public long SumInsured { get; set; }

        /// <summary>
        /// 退保金额
        /// </summary>
        [XmlElement("surrender_fee")]
        public long SurrenderFee { get; set; }

        /// <summary>
        /// 退保时间
        /// </summary>
        [XmlElement("surrender_time")]
        public string SurrenderTime { get; set; }
    }
}

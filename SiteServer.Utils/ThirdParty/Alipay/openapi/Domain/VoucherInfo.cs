using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// VoucherInfo Data Structure.
    /// </summary>
    [Serializable]
    public class VoucherInfo : AopObject
    {
        /// <summary>
        /// 是否可转赠。默认true
        /// </summary>
        [XmlElement("can_give_friend")]
        public bool CanGiveFriend { get; set; }

        /// <summary>
        /// 使用规则
        /// </summary>
        [XmlElement("use_rule")]
        public UseRuleInfo UseRule { get; set; }

        /// <summary>
        /// 有效期
        /// </summary>
        [XmlElement("valid_date")]
        public ValidDateInfo ValidDate { get; set; }

        /// <summary>
        /// 券详情描述
        /// </summary>
        [XmlElement("voucher_desc")]
        public string VoucherDesc { get; set; }

        /// <summary>
        /// 券背景图。该值调用接口:alipay.offline.material.image.upload生成
        /// </summary>
        [XmlElement("voucher_img")]
        public string VoucherImg { get; set; }

        /// <summary>
        /// logo图片id。该值调用接口:alipay.offline.material.image.upload生成
        /// </summary>
        [XmlElement("voucher_logo")]
        public string VoucherLogo { get; set; }

        /// <summary>
        /// 券名称
        /// </summary>
        [XmlElement("voucher_name")]
        public string VoucherName { get; set; }

        /// <summary>
        /// 券上的详情描述信息
        /// </summary>
        [XmlArray("voucher_terms")]
        [XmlArrayItem("voucher_term_info")]
        public List<VoucherTermInfo> VoucherTerms { get; set; }

        /// <summary>
        /// 券类型
        /// </summary>
        [XmlElement("voucher_type")]
        public string VoucherType { get; set; }
    }
}

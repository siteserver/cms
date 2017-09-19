using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// VoucherModifyInfo Data Structure.
    /// </summary>
    [Serializable]
    public class VoucherModifyInfo : AopObject
    {
        /// <summary>
        /// 追加的适用门店
        /// </summary>
        [XmlArray("suitable_shops")]
        [XmlArrayItem("string")]
        public List<string> SuitableShops { get; set; }

        /// <summary>
        /// 描述信息
        /// </summary>
        [XmlElement("voucher_desc")]
        public string VoucherDesc { get; set; }

        /// <summary>
        /// 券id
        /// </summary>
        [XmlElement("voucher_id")]
        public string VoucherId { get; set; }

        /// <summary>
        /// 券名称
        /// </summary>
        [XmlElement("voucher_name")]
        public string VoucherName { get; set; }

        /// <summary>
        /// 使用须知
        /// </summary>
        [XmlArray("voucher_terms")]
        [XmlArrayItem("voucher_term_info")]
        public List<VoucherTermInfo> VoucherTerms { get; set; }
    }
}

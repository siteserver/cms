using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ProdLORelationVO Data Structure.
    /// </summary>
    [Serializable]
    public class ProdLORelationVO : AopObject
    {
        /// <summary>
        /// 区域编码
        /// </summary>
        [XmlElement("lo_code")]
        public string LoCode { get; set; }

        /// <summary>
        /// 区域类型
        /// </summary>
        [XmlElement("lo_type")]
        public string LoType { get; set; }

        /// <summary>
        /// 产品码
        /// </summary>
        [XmlElement("prod_code")]
        public string ProdCode { get; set; }

        /// <summary>
        /// 产品版本
        /// </summary>
        [XmlElement("prod_version")]
        public string ProdVersion { get; set; }
    }
}

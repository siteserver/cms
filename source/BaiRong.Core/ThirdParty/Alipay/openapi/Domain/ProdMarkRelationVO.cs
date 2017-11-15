using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ProdMarkRelationVO Data Structure.
    /// </summary>
    [Serializable]
    public class ProdMarkRelationVO : AopObject
    {
        /// <summary>
        /// 产品外标编码
        /// </summary>
        [XmlElement("mark_code")]
        public string MarkCode { get; set; }

        /// <summary>
        /// 产品外标类型
        /// </summary>
        [XmlElement("mark_type")]
        public string MarkType { get; set; }

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

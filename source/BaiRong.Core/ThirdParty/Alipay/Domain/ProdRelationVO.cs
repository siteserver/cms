using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ProdRelationVO Data Structure.
    /// </summary>
    [Serializable]
    public class ProdRelationVO : AopObject
    {
        /// <summary>
        /// 数量
        /// </summary>
        [XmlElement("num")]
        public long Num { get; set; }

        /// <summary>
        /// 数量类型
        /// </summary>
        [XmlElement("num_type")]
        public string NumType { get; set; }

        /// <summary>
        /// 产品码
        /// </summary>
        [XmlElement("prod_code")]
        public string ProdCode { get; set; }

        /// <summary>
        /// 关联的产品编码
        /// </summary>
        [XmlElement("prod_rel_code")]
        public string ProdRelCode { get; set; }

        /// <summary>
        /// 关联的产品名称
        /// </summary>
        [XmlElement("prod_rel_name")]
        public string ProdRelName { get; set; }

        /// <summary>
        /// 产品关联类型
        /// </summary>
        [XmlElement("prod_rel_type")]
        public string ProdRelType { get; set; }

        /// <summary>
        /// 关联的产品版本
        /// </summary>
        [XmlElement("prod_rel_version")]
        public string ProdRelVersion { get; set; }

        /// <summary>
        /// 产品版本
        /// </summary>
        [XmlElement("prod_version")]
        public string ProdVersion { get; set; }

        /// <summary>
        /// 产品关联子类型
        /// </summary>
        [XmlElement("sub_prod_rel_type")]
        public string SubProdRelType { get; set; }
    }
}

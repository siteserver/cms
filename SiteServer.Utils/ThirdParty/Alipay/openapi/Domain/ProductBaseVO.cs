using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ProductBaseVO Data Structure.
    /// </summary>
    [Serializable]
    public class ProductBaseVO : AopObject
    {
        /// <summary>
        /// 产品业务状态,产品中心目前暂时不消费该业务状态，由外围业务消费
        /// </summary>
        [XmlElement("biz_status")]
        public string BizStatus { get; set; }

        /// <summary>
        /// 是否是组合产品,默认为false,填02
        /// </summary>
        [XmlElement("is_combinate")]
        public string IsCombinate { get; set; }

        /// <summary>
        /// 产品环境类型，默认是线上
        /// </summary>
        [XmlElement("prod_env")]
        public string ProdEnv { get; set; }

        /// <summary>
        /// 产品名称
        /// </summary>
        [XmlElement("prod_name")]
        public string ProdName { get; set; }

        /// <summary>
        /// 产品模板编码
        /// </summary>
        [XmlElement("prod_template_code")]
        public string ProdTemplateCode { get; set; }

        /// <summary>
        /// 产品模板版本
        /// </summary>
        [XmlElement("prod_template_version")]
        public string ProdTemplateVersion { get; set; }

        /// <summary>
        /// 产品类型 :01: 普通产品；02：标准产品
        /// </summary>
        [XmlElement("prod_type")]
        public string ProdType { get; set; }

        /// <summary>
        /// 产品编码
        /// </summary>
        [XmlElement("prod_version")]
        public string ProdVersion { get; set; }

        /// <summary>
        /// 产品管理状态
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }

        /// <summary>
        /// 标准产品编码
        /// </summary>
        [XmlElement("std_prod_code")]
        public string StdProdCode { get; set; }

        /// <summary>
        /// 标准产品版本
        /// </summary>
        [XmlElement("std_prod_version")]
        public string StdProdVersion { get; set; }
    }
}

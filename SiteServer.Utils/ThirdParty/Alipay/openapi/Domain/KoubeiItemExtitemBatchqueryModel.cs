using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiItemExtitemBatchqueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiItemExtitemBatchqueryModel : AopObject
    {
        /// <summary>
        /// 品牌编码
        /// </summary>
        [XmlElement("brand_code")]
        public string BrandCode { get; set; }

        /// <summary>
        /// 品类编码
        /// </summary>
        [XmlElement("category_code")]
        public string CategoryCode { get; set; }

        /// <summary>
        /// 当前页码。
        /// </summary>
        [XmlElement("page_num")]
        public string PageNum { get; set; }

        /// <summary>
        /// 分页大小。最大50条，超过限制默认50
        /// </summary>
        [XmlElement("page_size")]
        public string PageSize { get; set; }

        /// <summary>
        /// 商品名称（仅支持前缀匹配）
        /// </summary>
        [XmlElement("title")]
        public string Title { get; set; }
    }
}

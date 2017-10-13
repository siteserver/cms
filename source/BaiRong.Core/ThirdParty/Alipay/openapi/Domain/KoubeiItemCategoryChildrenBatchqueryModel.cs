using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiItemCategoryChildrenBatchqueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiItemCategoryChildrenBatchqueryModel : AopObject
    {
        /// <summary>
        /// 根类目ID. 参数非必传，不传该参数时查询所有的一级类目及递归子类目； 传该参数时，根据入参递归查询子类目信息的列表返回
        /// </summary>
        [XmlElement("root_category_id")]
        public string RootCategoryId { get; set; }
    }
}

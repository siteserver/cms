using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// InvestigCategoryResult Data Structure.
    /// </summary>
    [Serializable]
    public class InvestigCategoryResult : AopObject
    {
        /// <summary>
        /// 数据项Category
        /// </summary>
        [XmlElement("category")]
        public string Category { get; set; }

        /// <summary>
        /// 数据项对应的所有采集结果
        /// </summary>
        [XmlArray("category_result")]
        [XmlArrayItem("investig_category_data")]
        public List<InvestigCategoryData> CategoryResult { get; set; }
    }
}

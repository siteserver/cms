using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AntProdpaasArrangementRebateRateQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AntProdpaasArrangementRebateRateQueryModel : AopObject
    {
        /// <summary>
        /// 数据项名称
        /// </summary>
        [XmlElement("data_item_name")]
        public string DataItemName { get; set; }

        /// <summary>
        /// 一级类目ID
        /// </summary>
        [XmlElement("first_category_id")]
        public string FirstCategoryId { get; set; }

        /// <summary>
        /// 查询时间
        /// </summary>
        [XmlElement("gmt_query")]
        public string GmtQuery { get; set; }
    }
}

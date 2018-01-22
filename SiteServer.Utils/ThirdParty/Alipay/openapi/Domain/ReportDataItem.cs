using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ReportDataItem Data Structure.
    /// </summary>
    [Serializable]
    public class ReportDataItem : AopObject
    {
        /// <summary>
        /// 表示一行数据，每个对象是一列的数据
        /// </summary>
        [XmlElement("row_data")]
        public string RowData { get; set; }
    }
}

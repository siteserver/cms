using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// CustomerTag Data Structure.
    /// </summary>
    [Serializable]
    public class CustomerTag : AopObject
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        [XmlElement("col_name")]
        public string ColName { get; set; }

        /// <summary>
        /// column_type字段类型
        /// </summary>
        [XmlElement("column_type")]
        public string ColumnType { get; set; }

        /// <summary>
        /// id
        /// </summary>
        [XmlElement("id")]
        public string Id { get; set; }

        /// <summary>
        /// 标签名
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }
    }
}

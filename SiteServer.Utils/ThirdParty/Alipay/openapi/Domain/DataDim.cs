using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// DataDim Data Structure.
    /// </summary>
    [Serializable]
    public class DataDim : AopObject
    {
        /// <summary>
        /// 维度名称，代表维度层级含义  不同维度间用“|”分割
        /// </summary>
        [XmlElement("dim_name")]
        public string DimName { get; set; }

        /// <summary>
        /// 维度类型，并级或者层级  parallel     并列维度  hierarchical 层级维度
        /// </summary>
        [XmlElement("dim_type")]
        public string DimType { get; set; }

        /// <summary>
        /// 维度值，代表维度层级的值
        /// </summary>
        [XmlElement("dim_value")]
        public string DimValue { get; set; }
    }
}

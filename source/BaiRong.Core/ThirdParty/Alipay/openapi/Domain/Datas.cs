using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// Datas Data Structure.
    /// </summary>
    [Serializable]
    public class Datas : AopObject
    {
        /// <summary>
        /// 指标数据区
        /// </summary>
        [XmlArray("data")]
        [XmlArrayItem("data_entry")]
        public List<DataEntry> Data { get; set; }

        /// <summary>
        /// 数据维度
        /// </summary>
        [XmlArray("dimension")]
        [XmlArrayItem("data_dim")]
        public List<DataDim> Dimension { get; set; }
    }
}

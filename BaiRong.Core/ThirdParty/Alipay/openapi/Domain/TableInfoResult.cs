using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// TableInfoResult Data Structure.
    /// </summary>
    [Serializable]
    public class TableInfoResult : AopObject
    {
        /// <summary>
        /// 返回TableListResult集合
        /// </summary>
        [XmlArray("table_info_list")]
        [XmlArrayItem("table_list_result")]
        public List<TableListResult> TableInfoList { get; set; }
    }
}

using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayDataItemGoodsList Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayDataItemGoodsList : AopObject
    {
        /// <summary>
        /// 单品的描述信息
        /// </summary>
        [XmlElement("desc")]
        public string Desc { get; set; }

        /// <summary>
        /// 单品id列表
        /// </summary>
        [XmlArray("goods_list")]
        [XmlArrayItem("string")]
        public List<string> GoodsList { get; set; }
    }
}

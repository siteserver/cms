using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// SkuPropertyInfo Data Structure.
    /// </summary>
    [Serializable]
    public class SkuPropertyInfo : AopObject
    {
        /// <summary>
        /// sku属性key值,可发邮件到lei.mao@antfin.com,联系支付宝获取已支持的key.支付宝在收到邮件后三个工作日内回复
        /// </summary>
        [XmlElement("key")]
        public string Key { get; set; }

        /// <summary>
        /// 商家sku属性值列表
        /// </summary>
        [XmlArray("value")]
        [XmlArrayItem("sku_property_value")]
        public List<SkuPropertyValue> Value { get; set; }
    }
}

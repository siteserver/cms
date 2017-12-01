using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ModelMeta Data Structure.
    /// </summary>
    [Serializable]
    public class ModelMeta : AopObject
    {
        /// <summary>
        /// 模型描述
        /// </summary>
        [XmlElement("model_desc")]
        public string ModelDesc { get; set; }

        /// <summary>
        /// 模型名称
        /// </summary>
        [XmlElement("model_name")]
        public string ModelName { get; set; }

        /// <summary>
        /// 模型唯一查询标识符
        /// </summary>
        [XmlElement("model_uk")]
        public string ModelUk { get; set; }

        /// <summary>
        /// 查询参数
        /// </summary>
        [XmlArray("query_key")]
        [XmlArrayItem("string")]
        public List<string> QueryKey { get; set; }
    }
}

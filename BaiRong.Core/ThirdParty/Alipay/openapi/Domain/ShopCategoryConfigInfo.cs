using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ShopCategoryConfigInfo Data Structure.
    /// </summary>
    [Serializable]
    public class ShopCategoryConfigInfo : AopObject
    {
        /// <summary>
        /// 类目ID
        /// </summary>
        [XmlElement("id")]
        public string Id { get; set; }

        /// <summary>
        /// 是否是叶子节点
        /// </summary>
        [XmlElement("is_leaf")]
        public string IsLeaf { get; set; }

        /// <summary>
        /// 类目层级
        /// </summary>
        [XmlElement("level")]
        public string Level { get; set; }

        /// <summary>
        /// 类目层级路径
        /// </summary>
        [XmlElement("link")]
        public string Link { get; set; }

        /// <summary>
        /// 类目名称
        /// </summary>
        [XmlElement("nm")]
        public string Nm { get; set; }
    }
}

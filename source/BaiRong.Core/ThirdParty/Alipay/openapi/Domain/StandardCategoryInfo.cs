using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// StandardCategoryInfo Data Structure.
    /// </summary>
    [Serializable]
    public class StandardCategoryInfo : AopObject
    {
        /// <summary>
        /// 后台类目ID，类目信息的主键；商品类目ID，发布/修改商品的时候，需要填写商品所属的类目ID
        /// </summary>
        [XmlElement("category_id")]
        public string CategoryId { get; set; }

        /// <summary>
        /// 是否为叶子类目，商品只能够发布在叶子类目下
        /// </summary>
        [XmlElement("is_leaf")]
        public string IsLeaf { get; set; }

        /// <summary>
        /// 类目名称
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// 本类目的上一级类目ID，如果本类目是一级类目，上一级类目ID为空
        /// </summary>
        [XmlElement("parent_id")]
        public string ParentId { get; set; }

        /// <summary>
        /// 类目路径，递归父一级类目ID的列表，依次按照一级、二级、三级...顺序排列
        /// </summary>
        [XmlElement("path")]
        public string Path { get; set; }

        /// <summary>
        /// 类目所属一级类目的ID，若本类目是一级类目，值为本类目的ID
        /// </summary>
        [XmlElement("root_id")]
        public string RootId { get; set; }
    }
}

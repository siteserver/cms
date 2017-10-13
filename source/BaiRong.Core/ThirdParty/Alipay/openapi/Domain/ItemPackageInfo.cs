using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ItemPackageInfo Data Structure.
    /// </summary>
    [Serializable]
    public class ItemPackageInfo : AopObject
    {
        /// <summary>
        /// 商品详情-套餐内菜品信息列表
        /// </summary>
        [XmlArray("item_units")]
        [XmlArrayItem("item_unit_info")]
        public List<ItemUnitInfo> ItemUnits { get; set; }

        /// <summary>
        /// 商品详情-套餐标题。最多不超过15个汉字，30个字符
        /// </summary>
        [XmlElement("title")]
        public string Title { get; set; }
    }
}

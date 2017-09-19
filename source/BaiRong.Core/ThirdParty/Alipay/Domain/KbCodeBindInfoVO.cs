using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KbCodeBindInfoVO Data Structure.
    /// </summary>
    [Serializable]
    public class KbCodeBindInfoVO : AopObject
    {
        /// <summary>
        /// 商家餐桌摆放的区域名称（预留字段，暂不使用）
        /// </summary>
        [XmlElement("area_name")]
        public string AreaName { get; set; }

        /// <summary>
        /// 餐桌最大就餐人数（预留字段，暂不使用）
        /// </summary>
        [XmlElement("max_pepole_num")]
        public long MaxPepoleNum { get; set; }

        /// <summary>
        /// 餐桌就餐的最少人数（预留字段，暂不使用）
        /// </summary>
        [XmlElement("min_pepole_num")]
        public string MinPepoleNum { get; set; }

        /// <summary>
        /// 口碑店铺ID
        /// </summary>
        [XmlElement("shop_id")]
        public string ShopId { get; set; }

        /// <summary>
        /// table_no对应的中文名称。（预留字段，暂不使用）
        /// </summary>
        [XmlElement("table_name")]
        public string TableName { get; set; }

        /// <summary>
        /// 商家收银系统录入的点菜桌号（生成桌码时，必填）
        /// </summary>
        [XmlElement("table_no")]
        public string TableNo { get; set; }
    }
}

using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KbCodeInfoVO Data Structure.
    /// </summary>
    [Serializable]
    public class KbCodeInfoVO : AopObject
    {
        /// <summary>
        /// 创建口碑码的批次号
        /// </summary>
        [XmlElement("batch_id")]
        public long BatchId { get; set; }

        /// <summary>
        /// 口碑码图片(不带背景图)
        /// </summary>
        [XmlElement("code_url")]
        public string CodeUrl { get; set; }

        /// <summary>
        /// 口碑码创建时间
        /// </summary>
        [XmlElement("create_time")]
        public string CreateTime { get; set; }

        /// <summary>
        /// 口碑码ID
        /// </summary>
        [XmlElement("qr_code")]
        public string QrCode { get; set; }

        /// <summary>
        /// 口碑码物料图（带背景）
        /// </summary>
        [XmlElement("resource_url")]
        public string ResourceUrl { get; set; }

        /// <summary>
        /// 口碑店铺ID
        /// </summary>
        [XmlElement("shop_id")]
        public string ShopId { get; set; }

        /// <summary>
        /// 口碑门店名称
        /// </summary>
        [XmlElement("shop_name")]
        public string ShopName { get; set; }

        /// <summary>
        /// 物料模板
        /// </summary>
        [XmlElement("stuff_template")]
        public string StuffTemplate { get; set; }

        /// <summary>
        /// 物料模板描述
        /// </summary>
        [XmlElement("stuff_template_desc")]
        public string StuffTemplateDesc { get; set; }

        /// <summary>
        /// 口碑码类型描述
        /// </summary>
        [XmlElement("stuff_type_desc")]
        public string StuffTypeDesc { get; set; }

        /// <summary>
        /// 桌号
        /// </summary>
        [XmlElement("table_no")]
        public string TableNo { get; set; }
    }
}

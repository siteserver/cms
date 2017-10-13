using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// CommodityExtInfoConfirm Data Structure.
    /// </summary>
    [Serializable]
    public class CommodityExtInfoConfirm : AopObject
    {
        /// <summary>
        /// 城市上架结果 【 0：表示失败，  1：表示成功】
        /// </summary>
        [XmlElement("city_status")]
        public string CityStatus { get; set; }

        /// <summary>
        /// 挂载ID，用于确认唯一记录的主键对象
        /// </summary>
        [XmlElement("displayapp_id")]
        public string DisplayappId { get; set; }

        /// <summary>
        /// 修改城市记录映射对应的原有的挂载id,如果原有服务没有上架城市，该参数为空
        /// </summary>
        [XmlElement("mapping_displayapp_id")]
        public string MappingDisplayappId { get; set; }

        /// <summary>
        /// 城市上架失败原因
        /// </summary>
        [XmlElement("memo")]
        public string Memo { get; set; }
    }
}

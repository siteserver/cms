using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// CommodityPublicExtInfos Data Structure.
    /// </summary>
    [Serializable]
    public class CommodityPublicExtInfos : AopObject
    {
        /// <summary>
        /// 前置url
        /// </summary>
        [XmlElement("action_url")]
        public string ActionUrl { get; set; }

        /// <summary>
        /// 应用展台id
        /// </summary>
        [XmlElement("app_id")]
        public string AppId { get; set; }

        /// <summary>
        /// 类目
        /// </summary>
        [XmlElement("category_name")]
        public string CategoryName { get; set; }

        /// <summary>
        /// 城市名称
        /// </summary>
        [XmlElement("city_name")]
        public string CityName { get; set; }

        /// <summary>
        /// 服务插件ID
        /// </summary>
        [XmlElement("commodity_id")]
        public string CommodityId { get; set; }

        /// <summary>
        /// 创建者ID
        /// </summary>
        [XmlElement("create_user_id")]
        public string CreateUserId { get; set; }

        /// <summary>
        /// 挂载ID，用于确认唯一记录的主键对象
        /// </summary>
        [XmlElement("displayapp_id")]
        public string DisplayappId { get; set; }

        /// <summary>
        /// 城市服务说明
        /// </summary>
        [XmlElement("displayapp_memo")]
        public string DisplayappMemo { get; set; }

        /// <summary>
        /// 服务别名
        /// </summary>
        [XmlElement("displayapp_name")]
        public string DisplayappName { get; set; }

        /// <summary>
        /// 状态 1:上架；0：下架；2:维护中
        /// </summary>
        [XmlElement("displayapp_status")]
        public string DisplayappStatus { get; set; }

        /// <summary>
        /// 用户访问地址
        /// </summary>
        [XmlElement("displayapp_url")]
        public string DisplayappUrl { get; set; }

        /// <summary>
        /// 外部展示地址
        /// </summary>
        [XmlElement("export_url")]
        public string ExportUrl { get; set; }

        /// <summary>
        /// 属性ID
        /// </summary>
        [XmlElement("property_id")]
        public string PropertyId { get; set; }
    }
}

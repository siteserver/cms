using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// MaintainVehicleInfo Data Structure.
    /// </summary>
    [Serializable]
    public class MaintainVehicleInfo : AopObject
    {
        /// <summary>
        /// 背景图片
        /// </summary>
        [XmlElement("bg_url")]
        public string BgUrl { get; set; }

        /// <summary>
        /// 发动机编号
        /// </summary>
        [XmlElement("engine_no")]
        public string EngineNo { get; set; }

        /// <summary>
        /// 发动机类型
        /// </summary>
        [XmlElement("engine_type")]
        public string EngineType { get; set; }

        /// <summary>
        /// 生产厂商
        /// </summary>
        [XmlElement("manufacturer")]
        public string Manufacturer { get; set; }

        /// <summary>
        /// 生产年份
        /// </summary>
        [XmlElement("production_year")]
        public string ProductionYear { get; set; }

        /// <summary>
        /// 发动机排量
        /// </summary>
        [XmlArray("swept_volume")]
        [XmlArrayItem("string")]
        public List<string> SweptVolume { get; set; }

        /// <summary>
        /// 品牌Logo
        /// </summary>
        [XmlElement("vi_brand_logo")]
        public string ViBrandLogo { get; set; }

        /// <summary>
        /// 品牌名称
        /// </summary>
        [XmlElement("vi_brand_name")]
        public string ViBrandName { get; set; }

        /// <summary>
        /// 车辆归属地id
        /// </summary>
        [XmlElement("vi_city_id")]
        public string ViCityId { get; set; }

        /// <summary>
        /// 城市名称
        /// </summary>
        [XmlElement("vi_city_name")]
        public string ViCityName { get; set; }

        /// <summary>
        /// 车辆图标URL
        /// </summary>
        [XmlElement("vi_logo_url")]
        public string ViLogoUrl { get; set; }

        /// <summary>
        /// 行驶里程
        /// </summary>
        [XmlElement("vi_mileage")]
        public string ViMileage { get; set; }

        /// <summary>
        /// 车型名称
        /// </summary>
        [XmlElement("vi_model_name")]
        public string ViModelName { get; set; }

        /// <summary>
        /// 车牌号
        /// </summary>
        [XmlElement("vi_number")]
        public string ViNumber { get; set; }

        /// <summary>
        /// 车系名称
        /// </summary>
        [XmlElement("vi_series_name")]
        public string ViSeriesName { get; set; }

        /// <summary>
        /// 上路日期
        /// </summary>
        [XmlElement("vi_start_time")]
        public string ViStartTime { get; set; }

        /// <summary>
        /// 年款
        /// </summary>
        [XmlElement("vi_style_name")]
        public string ViStyleName { get; set; }

        /// <summary>
        /// 行驶里程
        /// </summary>
        [XmlArray("vi_vin")]
        [XmlArrayItem("string")]
        public List<string> ViVin { get; set; }
    }
}

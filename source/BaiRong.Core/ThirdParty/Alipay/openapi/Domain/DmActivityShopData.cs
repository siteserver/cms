using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// DmActivityShopData Data Structure.
    /// </summary>
    [Serializable]
    public class DmActivityShopData : AopObject
    {
        /// <summary>
        /// 数据创建时间戳
        /// </summary>
        [XmlElement("gmt_create")]
        public string GmtCreate { get; set; }

        /// <summary>
        /// 数据修改时间戳
        /// </summary>
        [XmlElement("gmt_modified")]
        public string GmtModified { get; set; }

        /// <summary>
        /// 单日点击人数
        /// </summary>
        [XmlElement("one_day_click_persons")]
        public long OneDayClickPersons { get; set; }

        /// <summary>
        /// 单日点击次数
        /// </summary>
        [XmlElement("one_day_click_times")]
        public long OneDayClickTimes { get; set; }

        /// <summary>
        /// 单日点击人数
        /// </summary>
        [XmlElement("one_day_exposure_times")]
        public long OneDayExposureTimes { get; set; }

        /// <summary>
        /// 门店ID
        /// </summary>
        [XmlElement("shop_id")]
        public string ShopId { get; set; }

        /// <summary>
        /// 总点击人数
        /// </summary>
        [XmlElement("total_click_persons")]
        public long TotalClickPersons { get; set; }

        /// <summary>
        /// 总点击次数
        /// </summary>
        [XmlElement("total_click_times")]
        public long TotalClickTimes { get; set; }

        /// <summary>
        /// 总曝光次数
        /// </summary>
        [XmlElement("total_exposure_times")]
        public long TotalExposureTimes { get; set; }
    }
}

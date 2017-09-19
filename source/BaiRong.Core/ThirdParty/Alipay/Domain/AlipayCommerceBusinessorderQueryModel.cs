using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayCommerceBusinessorderQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayCommerceBusinessorderQueryModel : AopObject
    {
        /// <summary>
        /// 查询办事记录的时间区间中的开始时间，格式为yyyy-MM-dd HH:mm:ss
        /// </summary>
        [XmlElement("begin_time")]
        public string BeginTime { get; set; }

        /// <summary>
        /// 查询办事记录的时间区间中的结束时间，格式为yyyy-MM-dd HH:mm:ss
        /// </summary>
        [XmlElement("end_time")]
        public string EndTime { get; set; }

        /// <summary>
        /// isv的appid
        /// </summary>
        [XmlElement("isv_appid")]
        public string IsvAppid { get; set; }

        /// <summary>
        /// 分页查询的起始页数
        /// </summary>
        [XmlElement("page_num")]
        public string PageNum { get; set; }

        /// <summary>
        /// 分页查询的每页数据量
        /// </summary>
        [XmlElement("page_size")]
        public string PageSize { get; set; }

        /// <summary>
        /// 查询的办事记录所属服务展台（如城市服务为CITY_SERVICE，车主平台为MYCAR_SERVICE等）
        /// </summary>
        [XmlElement("platform_type")]
        public string PlatformType { get; set; }

        /// <summary>
        /// 办事记录状态列表
        /// </summary>
        [XmlArray("status_list")]
        [XmlArrayItem("string")]
        public List<string> StatusList { get; set; }

        /// <summary>
        /// 支付宝userId
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}

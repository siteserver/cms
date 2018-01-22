using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// FengdieActivityCreateData Data Structure.
    /// </summary>
    [Serializable]
    public class FengdieActivityCreateData : AopObject
    {
        /// <summary>
        /// H5应用的名称，用户自定义，最终用于生成URL。生成URL的规则“域名/p/f/${name}/页面名称.html”
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// 用户自定义，到了该时间后，用户将访问不到该应用
        /// </summary>
        [XmlElement("offline_time")]
        public string OfflineTime { get; set; }

        /// <summary>
        /// H5应用的页面在编辑器中默认展示的数据
        /// </summary>
        [XmlArray("page")]
        [XmlArrayItem("fengdie_activity_create_page_data")]
        public List<FengdieActivityCreatePageData> Page { get; set; }

        /// <summary>
        /// H5应用标题，用户自定义
        /// </summary>
        [XmlElement("title")]
        public string Title { get; set; }
    }
}

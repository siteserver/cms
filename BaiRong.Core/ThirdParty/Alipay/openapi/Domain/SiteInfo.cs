using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// SiteInfo Data Structure.
    /// </summary>
    [Serializable]
    public class SiteInfo : AopObject
    {
        /// <summary>
        /// 测试账号
        /// </summary>
        [XmlElement("account")]
        public string Account { get; set; }

        /// <summary>
        /// 测试密码
        /// </summary>
        [XmlElement("password")]
        public string Password { get; set; }

        /// <summary>
        /// 站点名称
        /// </summary>
        [XmlElement("site_name")]
        public string SiteName { get; set; }

        /// <summary>
        /// 网站：01  APP  : 02  服务窗:03  公众号:04  其他:05
        /// </summary>
        [XmlElement("site_type")]
        public string SiteType { get; set; }

        /// <summary>
        /// 站点地址
        /// </summary>
        [XmlElement("site_url")]
        public string SiteUrl { get; set; }
    }
}

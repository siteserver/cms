using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayOpenAppQrcodeCreateModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayOpenAppQrcodeCreateModel : AopObject
    {
        /// <summary>
        /// 对应的二维码描述
        /// </summary>
        [XmlElement("describe")]
        public string Describe { get; set; }

        /// <summary>
        /// 小程序的启动参数，打开小程序的query ，在小程序 onLaunch的方法中获取
        /// </summary>
        [XmlElement("query_param")]
        public string QueryParam { get; set; }

        /// <summary>
        /// page/component/component-pages/view/view为小程序中能访问到的页面路径
        /// </summary>
        [XmlElement("url_param")]
        public string UrlParam { get; set; }
    }
}

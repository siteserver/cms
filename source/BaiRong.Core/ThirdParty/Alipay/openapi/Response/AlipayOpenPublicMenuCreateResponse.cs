using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayOpenPublicMenuCreateResponse.
    /// </summary>
    public class AlipayOpenPublicMenuCreateResponse : AopResponse
    {
        /// <summary>
        /// 默认菜单菜单key，文本菜单为“default”，icon菜单为“iconDefault”
        /// </summary>
        [XmlElement("menu_key")]
        public string MenuKey { get; set; }
    }
}

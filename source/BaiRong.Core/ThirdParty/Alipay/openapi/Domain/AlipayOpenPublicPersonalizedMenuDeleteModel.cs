using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayOpenPublicPersonalizedMenuDeleteModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayOpenPublicPersonalizedMenuDeleteModel : AopObject
    {
        /// <summary>
        /// 要删除的个性化菜单key
        /// </summary>
        [XmlElement("menu_key")]
        public string MenuKey { get; set; }
    }
}

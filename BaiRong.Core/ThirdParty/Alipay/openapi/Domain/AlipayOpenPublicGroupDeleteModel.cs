using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayOpenPublicGroupDeleteModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayOpenPublicGroupDeleteModel : AopObject
    {
        /// <summary>
        /// 需要删除的用户分组的id
        /// </summary>
        [XmlElement("group_id")]
        public string GroupId { get; set; }
    }
}

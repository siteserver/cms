using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayOpenPublicLifeModifyResponse.
    /// </summary>
    public class AlipayOpenPublicLifeModifyResponse : AopResponse
    {
        /// <summary>
        /// 修改时间。
        /// </summary>
        [XmlElement("modify_time")]
        public string ModifyTime { get; set; }

        /// <summary>
        /// 生活号id，用于表示此生活号唯一性
        /// </summary>
        [XmlElement("public_id")]
        public string PublicId { get; set; }
    }
}

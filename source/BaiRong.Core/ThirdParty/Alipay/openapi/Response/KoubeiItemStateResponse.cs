using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// KoubeiItemStateResponse.
    /// </summary>
    public class KoubeiItemStateResponse : AopResponse
    {
        /// <summary>
        /// 口碑体系内部商品的唯一标识
        /// </summary>
        [XmlElement("item_id")]
        public string ItemId { get; set; }

        /// <summary>
        /// 即入参中的request_id
        /// </summary>
        [XmlElement("request_id")]
        public string RequestId { get; set; }
    }
}

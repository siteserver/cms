using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// KoubeiItemCreateResponse.
    /// </summary>
    public class KoubeiItemCreateResponse : AopResponse
    {
        /// <summary>
        /// 口碑体系内部商品的唯一标识，后续的增删改查接口都使用该ID作为主键
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

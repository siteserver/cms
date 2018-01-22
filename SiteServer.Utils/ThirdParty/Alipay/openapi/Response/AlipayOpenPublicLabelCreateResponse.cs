using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayOpenPublicLabelCreateResponse.
    /// </summary>
    public class AlipayOpenPublicLabelCreateResponse : AopResponse
    {
        /// <summary>
        /// 支付宝返回的标签ID，开发者后续通过该ID进行标签的查询、修改、删除等操作
        /// </summary>
        [XmlElement("id")]
        public long Id { get; set; }

        /// <summary>
        /// 开发者传入name的值
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }
    }
}

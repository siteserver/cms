using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KbAdvertAddChannelRequest Data Structure.
    /// </summary>
    [Serializable]
    public class KbAdvertAddChannelRequest : AopObject
    {
        /// <summary>
        /// 描述信息(页面上不展现)
        /// </summary>
        [XmlElement("memo")]
        public string Memo { get; set; }

        /// <summary>
        /// 渠道名称
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// 类型可以通过koubei.advert.data.conf.query查询  OFFLINE：线下推广
        /// </summary>
        [XmlElement("type")]
        public string Type { get; set; }
    }
}

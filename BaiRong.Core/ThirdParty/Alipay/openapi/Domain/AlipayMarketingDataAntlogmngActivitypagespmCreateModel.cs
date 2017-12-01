using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayMarketingDataAntlogmngActivitypagespmCreateModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayMarketingDataAntlogmngActivitypagespmCreateModel : AopObject
    {
        /// <summary>
        /// 活动Id
        /// </summary>
        [XmlElement("activity_id")]
        public string ActivityId { get; set; }

        /// <summary>
        /// 负责人的工号
        /// </summary>
        [XmlElement("owner")]
        public string Owner { get; set; }

        /// <summary>
        /// spma位
        /// </summary>
        [XmlElement("spma")]
        public string Spma { get; set; }

        /// <summary>
        /// 页面的spmb值code
        /// </summary>
        [XmlElement("spmb")]
        public string Spmb { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [XmlElement("title")]
        public string Title { get; set; }

        /// <summary>
        /// 凤蝶页面的url
        /// </summary>
        [XmlElement("url")]
        public string Url { get; set; }
    }
}

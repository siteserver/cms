using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiMarketingAdvertisingOperateModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiMarketingAdvertisingOperateModel : AopObject
    {
        /// <summary>
        /// 广告ID，唯一标识一条广告
        /// </summary>
        [XmlElement("ad_id")]
        public string AdId { get; set; }

        /// <summary>
        /// 操作类型，目前包括上线和下线，分别传入：online(ONLINE)和offline(OFFLINE)
        /// </summary>
        [XmlElement("operate_type")]
        public string OperateType { get; set; }
    }
}

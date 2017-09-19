using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// CardInfo Data Structure.
    /// </summary>
    [Serializable]
    public class CardInfo : AopObject
    {
        /// <summary>
        /// 领取时间
        /// </summary>
        [XmlElement("taken_time")]
        public string TakenTime { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [XmlElement("user_name")]
        public string UserName { get; set; }
    }
}

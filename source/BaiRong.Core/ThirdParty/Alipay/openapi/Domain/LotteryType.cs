using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// LotteryType Data Structure.
    /// </summary>
    [Serializable]
    public class LotteryType : AopObject
    {
        /// <summary>
        /// 彩种ID
        /// </summary>
        [XmlElement("lottery_type_id")]
        public long LotteryTypeId { get; set; }

        /// <summary>
        /// 彩种名称
        /// </summary>
        [XmlElement("lottery_type_name")]
        public string LotteryTypeName { get; set; }
    }
}

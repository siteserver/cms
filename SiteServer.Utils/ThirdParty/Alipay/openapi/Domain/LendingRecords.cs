using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// LendingRecords Data Structure.
    /// </summary>
    [Serializable]
    public class LendingRecords : AopObject
    {
        /// <summary>
        /// 放款时间，精确到天
        /// </summary>
        [XmlElement("date")]
        public string Date { get; set; }

        /// <summary>
        /// 放款流水描述
        /// </summary>
        [XmlElement("remark")]
        public string Remark { get; set; }

        /// <summary>
        /// 放款额度，精确到小数点2位，单位（元）
        /// </summary>
        [XmlElement("total_amount")]
        public string TotalAmount { get; set; }
    }
}

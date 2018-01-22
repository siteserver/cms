using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KbAdvertSettleBillResponse Data Structure.
    /// </summary>
    [Serializable]
    public class KbAdvertSettleBillResponse : AopObject
    {
        /// <summary>
        /// 账单下载地址(为空表示查无账单)
        /// </summary>
        [XmlElement("download_url")]
        public string DownloadUrl { get; set; }

        /// <summary>
        /// 结算账单打款日期(为空表示未打款)
        /// </summary>
        [XmlElement("paid_date")]
        public string PaidDate { get; set; }
    }
}

using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayCommerceLotteryPresentlistQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayCommerceLotteryPresentlistQueryModel : AopObject
    {
        /// <summary>
        /// 结束日期，格式为yyyyMMdd
        /// </summary>
        [XmlElement("gmt_end")]
        public string GmtEnd { get; set; }

        /// <summary>
        /// 开始日期，格式为yyyyMMdd
        /// </summary>
        [XmlElement("gmt_start")]
        public string GmtStart { get; set; }

        /// <summary>
        /// 页号，必须大于0，默认为1
        /// </summary>
        [XmlElement("page_no")]
        public long PageNo { get; set; }

        /// <summary>
        /// 页大小，必须大于0，最大为500，默认为500
        /// </summary>
        [XmlElement("page_size")]
        public long PageSize { get; set; }
    }
}

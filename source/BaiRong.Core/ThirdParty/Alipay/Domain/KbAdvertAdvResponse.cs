using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KbAdvertAdvResponse Data Structure.
    /// </summary>
    [Serializable]
    public class KbAdvertAdvResponse : AopObject
    {
        /// <summary>
        /// 推广ID
        /// </summary>
        [XmlElement("adv_id")]
        public string AdvId { get; set; }

        /// <summary>
        /// 指定推广活动的名称
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// 单张券推广  只有type=single_voucher才会有值
        /// </summary>
        [XmlElement("single_voucher")]
        public KbAdvertAdvSingleVoucherResponse SingleVoucher { get; set; }

        /// <summary>
        /// 推广类型
        /// </summary>
        [XmlElement("type")]
        public string Type { get; set; }
    }
}

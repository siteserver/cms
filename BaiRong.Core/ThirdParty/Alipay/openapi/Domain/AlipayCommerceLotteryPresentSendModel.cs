using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayCommerceLotteryPresentSendModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayCommerceLotteryPresentSendModel : AopObject
    {
        /// <summary>
        /// 被赠送彩票的支付宝用户的ID，不支持一次赠送给多个用户
        /// </summary>
        [XmlElement("alipay_user_id")]
        public string AlipayUserId { get; set; }

        /// <summary>
        /// 彩种ID
        /// </summary>
        [XmlElement("lottery_type_id")]
        public long LotteryTypeId { get; set; }

        /// <summary>
        /// 外部订单号，不超过255字符，可包含英文和数字，需保证在商户端不重复
        /// </summary>
        [XmlElement("out_order_no")]
        public string OutOrderNo { get; set; }

        /// <summary>
        /// 彩票注数，大于0，最大为100
        /// </summary>
        [XmlElement("stake_count")]
        public long StakeCount { get; set; }

        /// <summary>
        /// 赠言，不超过20个汉字
        /// </summary>
        [XmlElement("swety_words")]
        public string SwetyWords { get; set; }
    }
}

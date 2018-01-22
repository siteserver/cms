using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEbppRechargeNotifySendModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEbppRechargeNotifySendModel : AopObject
    {
        /// <summary>
        /// 充值面额或者优惠面额
        /// </summary>
        [XmlElement("face_price")]
        public string FacePrice { get; set; }

        /// <summary>
        /// 充值时间或者话费券有效时间
        /// </summary>
        [XmlElement("gmt_charge")]
        public string GmtCharge { get; set; }

        /// <summary>
        /// 充值的手机号码
        /// </summary>
        [XmlElement("mobile")]
        public string Mobile { get; set; }

        /// <summary>
        /// 阿里通信通知类型
        /// </summary>
        [XmlElement("notify_type")]
        public string NotifyType { get; set; }

        /// <summary>
        /// 外部用户id
        /// </summary>
        [XmlElement("out_user_id")]
        public string OutUserId { get; set; }

        /// <summary>
        /// 充值交易号
        /// </summary>
        [XmlElement("trade_no")]
        public string TradeNo { get; set; }

        /// <summary>
        /// 支付宝用户id
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}

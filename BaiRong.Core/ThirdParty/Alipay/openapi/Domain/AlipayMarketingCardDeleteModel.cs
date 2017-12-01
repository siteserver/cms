using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayMarketingCardDeleteModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayMarketingCardDeleteModel : AopObject
    {
        /// <summary>
        /// 删卡扩展参数，json格式。  用于商户的特定业务信息的传递，只有商户与支付宝约定了传递此参数且约定了参数含义，此参数才有效。  目前支持如下key：  new_card_no：新卡号  donee_user_id：受赠人userId
        /// </summary>
        [XmlElement("ext_info")]
        public string ExtInfo { get; set; }

        /// <summary>
        /// 商户端删卡业务流水号（商户确保流水号唯一性）
        /// </summary>
        [XmlElement("out_serial_no")]
        public string OutSerialNo { get; set; }

        /// <summary>
        /// 删卡原因  USER_UNBUND：用户解绑（可以重新绑定）  CANCEL：销户（完成销户后，就不能再重新绑定）  PRESENT：转赠（可以重新绑定）
        /// </summary>
        [XmlElement("reason_code")]
        public string ReasonCode { get; set; }

        /// <summary>
        /// 支付宝业务卡号，开卡接口中返回获取
        /// </summary>
        [XmlElement("target_card_no")]
        public string TargetCardNo { get; set; }

        /// <summary>
        /// 卡号ID类型  BIZ_CARD：支付宝卡号
        /// </summary>
        [XmlElement("target_card_no_type")]
        public string TargetCardNoType { get; set; }
    }
}

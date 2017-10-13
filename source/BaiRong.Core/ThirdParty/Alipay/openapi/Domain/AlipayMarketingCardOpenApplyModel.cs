using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayMarketingCardOpenApplyModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayMarketingCardOpenApplyModel : AopObject
    {
        /// <summary>
        /// 外部卡信息(biz_card_no无需填写)
        /// </summary>
        [XmlElement("card_ext_info")]
        public MerchantCard CardExtInfo { get; set; }

        /// <summary>
        /// 支付宝分配的卡模板Id（卡模板创建接口返回的模板ID）
        /// </summary>
        [XmlElement("card_template_id")]
        public string CardTemplateId { get; set; }

        /// <summary>
        /// 发卡用户信息
        /// </summary>
        [XmlElement("card_user_info")]
        public CardUserInfo CardUserInfo { get; set; }

        /// <summary>
        /// 商户会员信息
        /// </summary>
        [XmlElement("member_ext_info")]
        public MerchantMenber MemberExtInfo { get; set; }

        /// <summary>
        /// 外部商户流水号（商户需要确保唯一性控制，类似request_id唯一请求标识）
        /// </summary>
        [XmlElement("out_serial_no")]
        public string OutSerialNo { get; set; }
    }
}

using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayMarketingCardOpenModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayMarketingCardOpenModel : AopObject
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
        /// 外部商户的领卡渠道，用于记录外部商户端领卡来源的渠道信息，渠道值可自行定义（仅限数字、字母、下划线）   例如：  线下门店领取:20161534000000000008863（可填门店shopId）  线下扫二维码领取:QR;  线下活动领取:20170522000000000003609（可填商户活动ID）
        /// </summary>
        [XmlElement("open_card_channel")]
        public string OpenCardChannel { get; set; }

        /// <summary>
        /// 外部商户流水号（商户需要确保唯一性控制，类似request_id唯一请求标识）
        /// </summary>
        [XmlElement("out_serial_no")]
        public string OutSerialNo { get; set; }
    }
}

using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayMarketingCardQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayMarketingCardQueryModel : AopObject
    {
        /// <summary>
        /// 用户信息 填写则作为附加条件查询
        /// </summary>
        [XmlElement("card_user_info")]
        public CardUserInfo CardUserInfo { get; set; }

        /// <summary>
        /// 扩展信息，暂时没有
        /// </summary>
        [XmlElement("ext_info")]
        public string ExtInfo { get; set; }

        /// <summary>
        /// 操作卡号。  target_card_no为业务卡号，由开卡流程中，支付宝返回的业务卡号
        /// </summary>
        [XmlElement("target_card_no")]
        public string TargetCardNo { get; set; }

        /// <summary>
        /// 卡号ID类型（会员卡查询，只能提供支付宝端的卡号）  BIZ_CARD：支付宝卡号  D_QR_CODE：动态二维码（业务卡号对应的）  D_BAR_CODE：动态条码（业务卡号对应的）  如果卡号不空，则类型不能为空
        /// </summary>
        [XmlElement("target_card_no_type")]
        public string TargetCardNoType { get; set; }
    }
}

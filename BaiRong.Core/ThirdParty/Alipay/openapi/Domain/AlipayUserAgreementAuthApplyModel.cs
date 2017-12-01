using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayUserAgreementAuthApplyModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayUserAgreementAuthApplyModel : AopObject
    {
        /// <summary>
        /// 支付宝系统中用以唯一标识用户签约记录的编号。
        /// </summary>
        [XmlElement("agreement_no")]
        public string AgreementNo { get; set; }

        /// <summary>
        /// 支付宝给用户下发短信校验码;  用户在商户提供页面中回填该校验码，商户调支付宝的鉴权确认接口，完全最终的鉴权确认
        /// </summary>
        [XmlElement("auth_confirm_type")]
        public string AuthConfirmType { get; set; }

        /// <summary>
        /// 鉴权申请的场景，目前可传入的值：AUTH/PAY
        /// </summary>
        [XmlElement("auth_scene")]
        public string AuthScene { get; set; }
    }
}

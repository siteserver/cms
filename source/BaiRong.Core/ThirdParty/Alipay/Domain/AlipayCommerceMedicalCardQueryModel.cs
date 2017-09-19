using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayCommerceMedicalCardQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayCommerceMedicalCardQueryModel : AopObject
    {
        /// <summary>
        /// 支付授权码
        /// </summary>
        [XmlElement("auth_code")]
        public string AuthCode { get; set; }

        /// <summary>
        /// 买家支付宝账号对应的支付宝唯一用户号。  以2088开头的纯16位数字。
        /// </summary>
        [XmlElement("buyer_id")]
        public string BuyerId { get; set; }

        /// <summary>
        /// 卡颁发机构编号
        /// </summary>
        [XmlElement("card_org_no")]
        public string CardOrgNo { get; set; }

        /// <summary>
        /// 业务扩展参数
        /// </summary>
        [XmlElement("extend_params")]
        public string ExtendParams { get; set; }

        /// <summary>
        /// 跳回的地址
        /// </summary>
        [XmlElement("return_url")]
        public string ReturnUrl { get; set; }

        /// <summary>
        /// 支付场景  条码支付，取值：bar_code  声波支付，取值：wave_code
        /// </summary>
        [XmlElement("scene")]
        public string Scene { get; set; }
    }
}

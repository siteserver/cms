using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// InvoiceTitleQueryOpenModel Data Structure.
    /// </summary>
    [Serializable]
    public class InvoiceTitleQueryOpenModel : AopObject
    {
        /// <summary>
        /// 购买方地址
        /// </summary>
        [XmlElement("payer_address")]
        public string PayerAddress { get; set; }

        /// <summary>
        /// 开户行账户
        /// </summary>
        [XmlElement("payer_bank_account")]
        public string PayerBankAccount { get; set; }

        /// <summary>
        /// 购买方开户银行
        /// </summary>
        [XmlElement("payer_bank_name")]
        public string PayerBankName { get; set; }

        /// <summary>
        /// 购买方纳税人识别号
        /// </summary>
        [XmlElement("payer_register_no")]
        public string PayerRegisterNo { get; set; }

        /// <summary>
        /// 购买方电话
        /// </summary>
        [XmlElement("payer_tel")]
        public string PayerTel { get; set; }

        /// <summary>
        /// 发票抬头名称
        /// </summary>
        [XmlElement("title_name")]
        public string TitleName { get; set; }

        /// <summary>
        /// 支付宝用户id，支付宝用户的唯一标识。
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}

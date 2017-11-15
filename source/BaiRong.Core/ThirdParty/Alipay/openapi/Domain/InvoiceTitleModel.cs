using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// InvoiceTitleModel Data Structure.
    /// </summary>
    [Serializable]
    public class InvoiceTitleModel : AopObject
    {
        /// <summary>
        /// 是否默认  可选值：  false：非默认  true：默认抬头
        /// </summary>
        [XmlElement("is_default")]
        public bool IsDefault { get; set; }

        /// <summary>
        /// 支付宝用户登录名（脱敏后登录名）  该字段输出接口只限  alipay.ebpp.invoice.title.dynamic.get
        /// </summary>
        [XmlElement("logon_id")]
        public string LogonId { get; set; }

        /// <summary>
        /// 开户行账号
        /// </summary>
        [XmlElement("open_bank_account")]
        public string OpenBankAccount { get; set; }

        /// <summary>
        /// 开户行
        /// </summary>
        [XmlElement("open_bank_name")]
        public string OpenBankName { get; set; }

        /// <summary>
        /// 纳税人识别号
        /// </summary>
        [XmlElement("tax_register_no")]
        public string TaxRegisterNo { get; set; }

        /// <summary>
        /// 发票抬头名称
        /// </summary>
        [XmlElement("title_name")]
        public string TitleName { get; set; }

        /// <summary>
        /// 发票类型  可选值：  PERSONAL（个人抬头）  CORPORATION（公司抬头）
        /// </summary>
        [XmlElement("title_type")]
        public string TitleType { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [XmlElement("user_address")]
        public string UserAddress { get; set; }

        /// <summary>
        /// 用户邮箱
        /// </summary>
        [XmlElement("user_email")]
        public string UserEmail { get; set; }

        /// <summary>
        /// 支付宝用户id  说明：动态码获取抬头时接口（alipay.ebpp.invoice.title.dynamic.get ）用户id返回结果为加密后密文  其他情况用户id来源于用户授权
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }

        /// <summary>
        /// 联系电话，支持手机和固话两种格式
        /// </summary>
        [XmlElement("user_mobile")]
        public string UserMobile { get; set; }
    }
}

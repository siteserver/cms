using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayUserAntpaasTestaccountCreateModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayUserAntpaasTestaccountCreateModel : AopObject
    {
        /// <summary>
        /// 认证等级，L1，L2，L3，L3可以开店
        /// </summary>
        [XmlElement("account_level")]
        public string AccountLevel { get; set; }

        /// <summary>
        /// 证件中的姓名，必须为中文，尽量不要超过6个汉字
        /// </summary>
        [XmlElement("cert_name")]
        public string CertName { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        [XmlElement("cert_no")]
        public string CertNo { get; set; }

        /// <summary>
        /// 证件类型，IDENTITY_CARD 身份证，PASSPORT 护照，HK_MC_CARD 港澳证件
        /// </summary>
        [XmlElement("cert_type")]
        public string CertType { get; set; }

        /// <summary>
        /// 登录名，如邮箱的值
        /// </summary>
        [XmlElement("logon_id")]
        public string LogonId { get; set; }

        /// <summary>
        /// 登录名类型，EMAIL
        /// </summary>
        [XmlElement("logon_type")]
        public string LogonType { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        [XmlElement("memo")]
        public string Memo { get; set; }

        /// <summary>
        /// 操作类型，CREATION 仅创建, CERTIFY 仅认证, CREATION_AND_CERTIFY 创建+认证
        /// </summary>
        [XmlElement("operation_type")]
        public string OperationType { get; set; }

        /// <summary>
        /// 操作者工号
        /// </summary>
        [XmlElement("operator_id")]
        public string OperatorId { get; set; }

        /// <summary>
        /// 调用方IP地址
        /// </summary>
        [XmlElement("remote_ip")]
        public string RemoteIp { get; set; }

        /// <summary>
        /// 支付宝账户id，accountNo
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }

        /// <summary>
        /// 账户状态，T Q
        /// </summary>
        [XmlElement("user_status")]
        public string UserStatus { get; set; }

        /// <summary>
        /// 账户类型，PERSON 个人 ORG 企业
        /// </summary>
        [XmlElement("user_type")]
        public string UserType { get; set; }
    }
}

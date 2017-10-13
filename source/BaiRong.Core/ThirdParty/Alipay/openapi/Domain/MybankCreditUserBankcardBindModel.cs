using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// MybankCreditUserBankcardBindModel Data Structure.
    /// </summary>
    [Serializable]
    public class MybankCreditUserBankcardBindModel : AopObject
    {
        /// <summary>
        /// 银行卡持有人的姓名
        /// </summary>
        [XmlElement("account_name")]
        public string AccountName { get; set; }

        /// <summary>
        /// 综管类型。目前只支持ALIPAY，业务方不需要理解该字段内容，按要求传值即可。
        /// </summary>
        [XmlElement("admin_type")]
        public string AdminType { get; set; }

        /// <summary>
        /// 综管账号ID。目前综管只支持ALIPAY，所以接入方理解成就传支付宝ID就可以
        /// </summary>
        [XmlElement("admin_user_id")]
        public string AdminUserId { get; set; }

        /// <summary>
        /// 需要绑定的银行卡的号码
        /// </summary>
        [XmlElement("bankcard_no")]
        public string BankcardNo { get; set; }

        /// <summary>
        /// 持卡人证件号码，证件类型为IDENTITY_CARD的时候，该值为身份证号码
        /// </summary>
        [XmlElement("cert_no")]
        public string CertNo { get; set; }

        /// <summary>
        /// 证件类型。持卡人的证件的类型，身份证传"IDENTITY_CARD"，目前只支持身份证
        /// </summary>
        [XmlElement("cert_type")]
        public string CertType { get; set; }

        /// <summary>
        /// 金融交换码  工商银行 ICBCC1CN  农业银行 AOBCC1CN  中国银行 BKCHC1CN  建设银行 PCBCC1CN  交通银行 BOCMC1CN  中信银行 ECITC1CN  广发银行 CGBCC1CN  民生银行 CMBCC1CN  光大银行 CEBBC1CN  平安银行 PINGC1CN  招商银行 CMBKC1CN  兴业银行 CIBKC1CN  浦发银行 SPDBC1CN  上海银行 SHHBC1CN  宁波银行 NBCBC1CN
        /// </summary>
        [XmlElement("fip_code")]
        public string FipCode { get; set; }

        /// <summary>
        /// 银行参与者id，是在网商银行创建会员后生成的id，网商银行会员的唯一标识
        /// </summary>
        [XmlElement("ip_id")]
        public string IpId { get; set; }

        /// <summary>
        /// 银行参与者角色id，是在网商银行创建会员后生成的角色id，网商银行会员角色的唯一标识
        /// </summary>
        [XmlElement("ip_role_id")]
        public string IpRoleId { get; set; }

        /// <summary>
        /// 该银行卡的用途。  还款账户：REPAYACCOUNT  放款账户：GRANTACCOUNT
        /// </summary>
        [XmlElement("purpose")]
        public string Purpose { get; set; }

        /// <summary>
        /// 外部流水号,唯一标识客户的一笔申请，做幂等性控制。格式：日期(8位)+序列号(8位）,序列号是数字，如00000001
        /// </summary>
        [XmlElement("request_id")]
        public string RequestId { get; set; }
    }
}

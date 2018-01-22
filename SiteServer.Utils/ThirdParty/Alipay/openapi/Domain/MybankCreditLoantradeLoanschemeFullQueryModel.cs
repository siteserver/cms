using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// MybankCreditLoantradeLoanschemeFullQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class MybankCreditLoantradeLoanschemeFullQueryModel : AopObject
    {
        /// <summary>
        /// 支付宝会员id，支付宝内部用于标识会员的唯一ID，以2088开头，不是支付宝登录账号
        /// </summary>
        [XmlElement("alipay_id")]
        public string AlipayId { get; set; }

        /// <summary>
        /// 支用金额，默认单位是人民币，精确到小数点两位，单位元
        /// </summary>
        [XmlElement("apply_amt")]
        public string ApplyAmt { get; set; }

        /// <summary>
        /// 申请支用日期(精度为天)，格式：YYYY-MM-DD，如：2017-01-01
        /// </summary>
        [XmlElement("apply_date")]
        public string ApplyDate { get; set; }

        /// <summary>
        /// 固化授信模式下的授信编号，由网商银行内部的系统生成，示例值：20161227BC2343C0000000001。若为预授信，则此值为空。
        /// </summary>
        [XmlElement("credit_no")]
        public string CreditNo { get; set; }

        /// <summary>
        /// 客群，信贷申请领域用来标识客户种类，由网商银行内部系统生成。
        /// </summary>
        [XmlElement("cust_group")]
        public string CustGroup { get; set; }

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
        /// 网商银行在对客户授信时，使用的政策产品唯一标识码，由网商银行内部生成，长度8位，字母和数字组成，示例值：BC32001C
        /// </summary>
        [XmlElement("loan_policy_code")]
        public string LoanPolicyCode { get; set; }

        /// <summary>
        /// 贷款期限，数值，在客户签署贷款合约时，会展示此值
        /// </summary>
        [XmlElement("loan_term")]
        public long LoanTerm { get; set; }

        /// <summary>
        /// 贷款期限单位，枚举值：Y、M、D，分别表示年月日，在客户签署贷款合约时，会展示此值
        /// </summary>
        [XmlElement("loan_term_unit")]
        public string LoanTermUnit { get; set; }

        /// <summary>
        /// 还款方式，枚举值：1（等额本息）、2（等额本金）、3（按期付息到期还本）、4（组合还款）、6（一次性到期还本付息）、7（固定利息等额分期），客户签署贷款合约时会展示此值
        /// </summary>
        [XmlElement("repay_mode")]
        public string RepayMode { get; set; }

        /// <summary>
        /// 销售产品码，一个信贷产品对外销售时的唯一标识，由网商银行内部分配，长度20位的一串数字，示例值：01021000100000000169
        /// </summary>
        [XmlElement("sale_pd_code")]
        public string SalePdCode { get; set; }

        /// <summary>
        /// 收款账号信息，注意：子参数不能全为空
        /// </summary>
        [XmlElement("trans_in_account")]
        public MyBkAccountVO TransInAccount { get; set; }
    }
}

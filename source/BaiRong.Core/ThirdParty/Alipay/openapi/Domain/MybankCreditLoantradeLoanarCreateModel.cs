using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// MybankCreditLoantradeLoanarCreateModel Data Structure.
    /// </summary>
    [Serializable]
    public class MybankCreditLoantradeLoanarCreateModel : AopObject
    {
        /// <summary>
        /// 收款账号外标  对于支付宝账户，支付宝loginId；  对于网商银行账户，是虚拟卡号；  对于外部银行卡，是银行卡号
        /// </summary>
        [XmlElement("account_ext_no")]
        public string AccountExtNo { get; set; }

        /// <summary>
        /// 收款方参与机构码，当收款账户为网商银行或外部银行时填充，即银行联行号（总行）
        /// </summary>
        [XmlElement("account_fin_code")]
        public string AccountFinCode { get; set; }

        /// <summary>
        /// 收款方机构名称
        /// </summary>
        [XmlElement("account_fin_name")]
        public string AccountFinName { get; set; }

        /// <summary>
        /// 帐户类别，对公/对私，枚举值如下：  1：对私   2：对公
        /// </summary>
        [XmlElement("account_fin_type")]
        public string AccountFinType { get; set; }

        /// <summary>
        /// 收款方名称
        /// </summary>
        [XmlElement("account_name")]
        public string AccountName { get; set; }

        /// <summary>
        /// 收款方账户号  对于支付宝账户，是支付宝会员id；  对于网商银行账户，是资金账号id；  对于外部银行卡，是银行卡号
        /// </summary>
        [XmlElement("account_no")]
        public string AccountNo { get; set; }

        /// <summary>
        /// 收款方账户类型，枚举值有：  ALIPAY（支付宝）  MY_BANK（网商银行）  OUT_BANK（外部银行）
        /// </summary>
        [XmlElement("account_type")]
        public string AccountType { get; set; }

        /// <summary>
        /// 本次发起支用的客户支付宝会员id，客户登录系统后由外部机构传入
        /// </summary>
        [XmlElement("alipay_id")]
        public string AlipayId { get; set; }

        /// <summary>
        /// 支用金额，默认人民币，精确到小数点两位，单位元
        /// </summary>
        [XmlElement("apply_amt")]
        public string ApplyAmt { get; set; }

        /// <summary>
        /// 外部业务单据号，在外部机构上发生的交易编号，如在菜鸟供销平台上发生的采购单的业务编号
        /// </summary>
        [XmlElement("bsn_no")]
        public string BsnNo { get; set; }

        /// <summary>
        /// 本次支用的授信编号，网商银行审批通过后回传给外部机构，然后由外部机构传入
        /// </summary>
        [XmlElement("credit_no")]
        public string CreditNo { get; set; }

        /// <summary>
        /// 客户群体，由网商银行事先分配给客户机构
        /// </summary>
        [XmlElement("cust_group")]
        public string CustGroup { get; set; }

        /// <summary>
        /// 放款账户渠道，从贷款方案查询接口获取  DEFAULT_ALIPAY 默认支付宝  ASSIGNED_ACCOUNT 定向支付账户  MYBANK_ACCOUNT 网商银行二类户
        /// </summary>
        [XmlElement("grant_channel")]
        public string GrantChannel { get; set; }

        /// <summary>
        /// 经营行业，枚举值如下：  F5199：零售  H6190：餐饮住宿  L7299：商业服务
        /// </summary>
        [XmlElement("industry")]
        public string Industry { get; set; }

        /// <summary>
        /// 网商银行参与者id，外部机构调用创建网商会员接口获取，参见接口：mybank.credit.user.role.create
        /// </summary>
        [XmlElement("ip_id")]
        public string IpId { get; set; }

        /// <summary>
        /// 网商银行参与者会员角色ID，外部机构调用创建网商会员接口获取，参见接口：mybank.credit.user.role.create
        /// </summary>
        [XmlElement("ip_role_id")]
        public string IpRoleId { get; set; }

        /// <summary>
        /// 本次支用的授信对应的BC政策码，由网商银行事先分配给外部机构
        /// </summary>
        [XmlElement("loan_policy_code")]
        public string LoanPolicyCode { get; set; }

        /// <summary>
        /// 贷款期限
        /// </summary>
        [XmlElement("loan_term")]
        public long LoanTerm { get; set; }

        /// <summary>
        /// 贷款期限单位，枚举值：  Y：年  M：月  D：日
        /// </summary>
        [XmlElement("loan_term_unit")]
        public string LoanTermUnit { get; set; }

        /// <summary>
        /// 客户签约的销售产品编码，由网商银行事先分配给外部机构
        /// </summary>
        [XmlElement("pd_code")]
        public string PdCode { get; set; }

        /// <summary>
        /// 客户签约的销售产品版本号，通过调用完整贷款方案（mybank.credit.loantrade.loanscheme.full.query）查询接口获取
        /// </summary>
        [XmlElement("pd_version")]
        public string PdVersion { get; set; }

        /// <summary>
        /// 还款方式，枚举值如下：  1：等额本息  2：等额本金  3：按期付息到期还本  4：组合还款  6：一次性到期还本付息  7：固定利息等额分期
        /// </summary>
        [XmlElement("repay_mode")]
        public string RepayMode { get; set; }

        /// <summary>
        /// 单次请求唯一流水号，用于幂等控制  结构为：客户机构ipRoleId_时间（日期、时分秒）_外部系统流水号
        /// </summary>
        [XmlElement("request_id")]
        public string RequestId { get; set; }

        /// <summary>
        /// 签名值
        /// </summary>
        [XmlElement("sign")]
        public string Sign { get; set; }

        /// <summary>
        /// 交易备注
        /// </summary>
        [XmlElement("trans_memo")]
        public string TransMemo { get; set; }
    }
}

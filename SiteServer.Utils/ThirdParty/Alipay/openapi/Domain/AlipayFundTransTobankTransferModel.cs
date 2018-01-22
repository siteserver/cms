using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayFundTransTobankTransferModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayFundTransTobankTransferModel : AopObject
    {
        /// <summary>
        /// 转账金额，单位：元。  只支持2位小数，小数点前最大支持13位，金额必须大于0。
        /// </summary>
        [XmlElement("amount")]
        public string Amount { get; set; }

        /// <summary>
        /// 用途信息。  当转账金额大于5万且银行卡账户类型为对私时，必须传递本参数。  当付款方为企业账户，且转账金额达到（大于等于）50000元，memo和remark不能同时为空。   1：代发工资协议和收款人清单   2：奖励   3：新闻版、演出等相关劳动合同   4：证券、期货、信托等退款   5：债权或产权转让协议   6：借款合同   7：保险合同   8：税收征管部门的   9：农、副、矿产品购销合同   10：其他合法款项的
        /// </summary>
        [XmlElement("memo")]
        public string Memo { get; set; }

        /// <summary>
        /// 商户转账唯一订单号：发起转账来源方定义的转账单据ID，用于将转账回执通知给来源方。  不同来源方给出的ID可以重复，同一个来源方必须保证其ID的唯一性。  只支持半角英文、数字，及“-”、“_”。
        /// </summary>
        [XmlElement("out_biz_no")]
        public string OutBizNo { get; set; }

        /// <summary>
        /// 收款方银行账户名，必须与收款方银行卡号所属账户信息一致。
        /// </summary>
        [XmlElement("payee_account_name")]
        public string PayeeAccountName { get; set; }

        /// <summary>
        /// 收款账户类型。   1：对公（在金融机构开设的公司账户）   2：对私（在金融机构开设的个人账户）
        /// </summary>
        [XmlElement("payee_account_type")]
        public string PayeeAccountType { get; set; }

        /// <summary>
        /// 收款支行联行号：收款银行支行联行号（支持32个英文/16个汉字）。
        /// </summary>
        [XmlElement("payee_bank_code")]
        public string PayeeBankCode { get; set; }

        /// <summary>
        /// 收款方银行卡号信息，只支持半角英文、数字及“-”。  目前只支持借记卡卡号。
        /// </summary>
        [XmlElement("payee_card_no")]
        public string PayeeCardNo { get; set; }

        /// <summary>
        /// 收款银行所属支行（支持100个英文/50个汉字）。
        /// </summary>
        [XmlElement("payee_inst_branch_name")]
        public string PayeeInstBranchName { get; set; }

        /// <summary>
        /// 收款银行所在市（支持40个英文/20个汉字）。
        /// </summary>
        [XmlElement("payee_inst_city")]
        public string PayeeInstCity { get; set; }

        /// <summary>
        /// 收款卡开户银行（支持30个英文/15个汉字）。
        /// </summary>
        [XmlElement("payee_inst_name")]
        public string PayeeInstName { get; set; }

        /// <summary>
        /// 收款银行所在省份（支持20个英文/10个汉字）。
        /// </summary>
        [XmlElement("payee_inst_province")]
        public string PayeeInstProvince { get; set; }

        /// <summary>
        /// 付款方真实姓名（支持100个英文/50个汉字）。  如果不为空，则会校验该账户在支付宝登入的实名是否与付款方真实姓名一致。
        /// </summary>
        [XmlElement("payer_real_name")]
        public string PayerRealName { get; set; }

        /// <summary>
        /// 银行备注（支持100个英文/50个汉字），该信息将通过银行渠道发送给收款方。  当付款方为企业账户，且转账金额达到（大于等于）50000元，memo和remark不能同时为空。
        /// </summary>
        [XmlElement("remark")]
        public string Remark { get; set; }

        /// <summary>
        /// 申请到账时效。   T0：当日到账   T1：次日到账
        /// </summary>
        [XmlElement("time_liness")]
        public string TimeLiness { get; set; }
    }
}

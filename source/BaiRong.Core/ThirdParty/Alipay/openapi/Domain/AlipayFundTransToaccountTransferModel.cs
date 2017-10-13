using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayFundTransToaccountTransferModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayFundTransToaccountTransferModel : AopObject
    {
        /// <summary>
        /// 转账金额，单位：元。  只支持2位小数，小数点前最大支持13位，金额必须大于等于0.1元。   最大转账金额以实际签约的限额为准。
        /// </summary>
        [XmlElement("amount")]
        public string Amount { get; set; }

        /// <summary>
        /// 扩展参数，json字符串格式，目前仅支持的key：order_title：收款方转账账单标题。  用于商户的特定业务信息的传递，只有商户与支付宝约定了传递此参数且约定了参数含义，此参数才有效。
        /// </summary>
        [XmlElement("ext_param")]
        public string ExtParam { get; set; }

        /// <summary>
        /// 商户转账唯一订单号。发起转账来源方定义的转账单据ID，用于将转账回执通知给来源方。  不同来源方给出的ID可以重复，同一个来源方必须保证其ID的唯一性。  只支持半角英文、数字，及“-”、“_”。
        /// </summary>
        [XmlElement("out_biz_no")]
        public string OutBizNo { get; set; }

        /// <summary>
        /// 收款方账户。与payee_type配合使用。付款方和收款方不能是同一个账户。
        /// </summary>
        [XmlElement("payee_account")]
        public string PayeeAccount { get; set; }

        /// <summary>
        /// 收款方真实姓名（最长支持100个英文/50个汉字）。  如果本参数不为空，则会校验该账户在支付宝登记的实名是否与收款方真实姓名一致。
        /// </summary>
        [XmlElement("payee_real_name")]
        public string PayeeRealName { get; set; }

        /// <summary>
        /// 收款方账户类型。可取值：  1、ALIPAY_USERID：支付宝账号对应的支付宝唯一用户号。以2088开头的16位纯数字组成。  2、ALIPAY_LOGONID：支付宝登录号，支持邮箱和手机号格式。
        /// </summary>
        [XmlElement("payee_type")]
        public string PayeeType { get; set; }

        /// <summary>
        /// 付款方真实姓名（最长支持100个英文/50个汉字）。  如果本参数不为空，则会校验该账户在支付宝登记的实名是否与付款方真实姓名一致。
        /// </summary>
        [XmlElement("payer_real_name")]
        public string PayerRealName { get; set; }

        /// <summary>
        /// 付款方姓名（最长支持100个英文/50个汉字）。显示在收款方的账单详情页。如果该字段不传，则默认显示付款方的支付宝认证姓名或单位名称。
        /// </summary>
        [XmlElement("payer_show_name")]
        public string PayerShowName { get; set; }

        /// <summary>
        /// 转账备注（支持200个英文/100个汉字）。  当付款方为企业账户，且转账金额达到（大于等于）50000元，remark不能为空。收款方可见，会展示在收款用户的收支详情中。
        /// </summary>
        [XmlElement("remark")]
        public string Remark { get; set; }
    }
}

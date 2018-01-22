using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// DstCampRuleModel Data Structure.
    /// </summary>
    [Serializable]
    public class DstCampRuleModel : AopObject
    {
        /// <summary>
        /// 支付宝收银台:PC端:PC   安全支付端:WIRELESS_CLIENT   无线WAP端:WIRELESS_WAP   协议支付;AGREEMENTPAY
        /// </summary>
        [XmlElement("alipay_cashier")]
        public string AlipayCashier { get; set; }

        /// <summary>
        /// 优惠规则类型,主要有2种:账户优惠传 account、渠道优惠channel. 现在开放账户优惠account  默认请传account
        /// </summary>
        [XmlElement("discount_type")]
        public string DiscountType { get; set; }

        /// <summary>
        /// 规则id 新增不传，修改传
        /// </summary>
        [XmlElement("id")]
        public string Id { get; set; }

        /// <summary>
        /// 同一个支付宝账户享受优惠次数.0表示不限制
        /// </summary>
        [XmlElement("prize_count_per_account")]
        public string PrizeCountPerAccount { get; set; }

        /// <summary>
        /// 产品类型 支付宝交易:trade   支付宝账单中心:commonDeduct   支付宝转账中心:ttc   支付宝通用代扣:billcenter
        /// </summary>
        [XmlElement("product_type")]
        public string ProductType { get; set; }

        /// <summary>
        /// 立减规则，这个参数有支付宝运营小二提供给商户，录入
        /// </summary>
        [XmlElement("rule_uuid")]
        public string RuleUuid { get; set; }

        /// <summary>
        /// 关联的凭证id,这个参数商户调凭证创建接口返回凭证id 然后设置到这个值
        /// </summary>
        [XmlElement("voucher_id")]
        public string VoucherId { get; set; }
    }
}

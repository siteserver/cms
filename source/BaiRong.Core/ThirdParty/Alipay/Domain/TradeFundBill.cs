using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// TradeFundBill Data Structure.
    /// </summary>
    [Serializable]
    public class TradeFundBill : AopObject
    {
        /// <summary>
        /// 该支付工具类型所使用的金额
        /// </summary>
        [XmlElement("amount")]
        public string Amount { get; set; }

        /// <summary>
        /// 交易使用的资金渠道，详见 <a href="https://doc.open.alipay.com/doc2/detail?treeId=26&articleId=103259&docType=1">支付渠道列表</a>
        /// </summary>
        [XmlElement("fund_channel")]
        public string FundChannel { get; set; }

        /// <summary>
        /// 渠道所使用的资金类型,目前只在资金渠道(fund_channel)是银行卡渠道(BANKCARD)的情况下才返回该信息(DEBIT_CARD:借记卡,CREDIT_CARD:信用卡,MIXED_CARD:借贷合一卡)
        /// </summary>
        [XmlElement("fund_type")]
        public string FundType { get; set; }

        /// <summary>
        /// 渠道实际付款金额
        /// </summary>
        [XmlElement("real_amount")]
        public string RealAmount { get; set; }
    }
}

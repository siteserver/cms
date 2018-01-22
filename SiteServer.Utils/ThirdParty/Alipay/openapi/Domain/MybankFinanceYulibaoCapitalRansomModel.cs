using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// MybankFinanceYulibaoCapitalRansomModel Data Structure.
    /// </summary>
    [Serializable]
    public class MybankFinanceYulibaoCapitalRansomModel : AopObject
    {
        /// <summary>
        /// 赎回的金额，以分为单位，必须为正整数。如amount=123456表示赎回1234.56元的余利宝份额。
        /// </summary>
        [XmlElement("amount")]
        public long Amount { get; set; }

        /// <summary>
        /// 币种，CNY表示人民币，目前只支持人民币
        /// </summary>
        [XmlElement("currency")]
        public string Currency { get; set; }

        /// <summary>
        /// 基金代码，必填。目前默认填001529，代表余利宝。
        /// </summary>
        [XmlElement("fund_code")]
        public string FundCode { get; set; }

        /// <summary>
        /// 余利宝赎回流水号，用于幂等控制。流水号必须长度在30到40位之间，且仅能由数字、字母、字符“-”和字符“_”组成。建议使用UUID，如“c39c24f1-73e5-497d-9b1f-0f585ae192c1”，或者使用自定义的数字流水号，如“201608150000000000000000000000000001”。
        /// </summary>
        [XmlElement("out_biz_no")]
        public string OutBizNo { get; set; }

        /// <summary>
        /// 赎回模式，REALTIME表示实时，NOTREALTIME表示非实时赎回（T+1到账），仅支持这两种模式。实时赎回日累计金额小于等于500万，大于500万则要使用非实时赎回，选择非实时赎回且日累计金额小于等于500万则会自动转为实时。
        /// </summary>
        [XmlElement("ransom_mode")]
        public string RansomMode { get; set; }
    }
}

using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEcoEduKtBillingQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEcoEduKtBillingQueryModel : AopObject
    {
        /// <summary>
        /// Isv pid
        /// </summary>
        [XmlElement("isv_pid")]
        public string IsvPid { get; set; }

        /// <summary>
        /// ISV调用发送账单接口，返回给商户的order_no
        /// </summary>
        [XmlElement("out_trade_no")]
        public string OutTradeNo { get; set; }

        /// <summary>
        /// 学校支付宝pid
        /// </summary>
        [XmlElement("school_pid")]
        public string SchoolPid { get; set; }
    }
}

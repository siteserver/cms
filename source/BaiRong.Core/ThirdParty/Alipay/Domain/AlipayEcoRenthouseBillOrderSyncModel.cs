using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEcoRenthouseBillOrderSyncModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEcoRenthouseBillOrderSyncModel : AopObject
    {
        /// <summary>
        /// 账单条数1-50范围内，账单条数和账单明细数量必须一致
        /// </summary>
        [XmlElement("bill_number")]
        public string BillNumber { get; set; }

        /// <summary>
        /// 账单条数1-50范围内
        /// </summary>
        [XmlArray("bills")]
        [XmlArrayItem("alipay_eco_renthouse_bill")]
        public List<AlipayEcoRenthouseBill> Bills { get; set; }

        /// <summary>
        /// ka请求的唯一标志，长度64位以内字符串，仅限字母数字下划线组合。该标识作为业务调用的唯一标识，ka要保证其业务唯一性
        /// </summary>
        [XmlElement("trade_id")]
        public string TradeId { get; set; }
    }
}

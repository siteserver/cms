using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiSalesKbassetStuffProduceqrcodeBatchqueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiSalesKbassetStuffProduceqrcodeBatchqueryModel : AopObject
    {
        /// <summary>
        /// 口碑批次号
        /// </summary>
        [XmlElement("batch_id")]
        public string BatchId { get; set; }

        /// <summary>
        /// 每页容量，最小1，最大100
        /// </summary>
        [XmlElement("page_size")]
        public string PageSize { get; set; }

        /// <summary>
        /// 生产单号
        /// </summary>
        [XmlElement("produce_order_id")]
        public string ProduceOrderId { get; set; }
    }
}

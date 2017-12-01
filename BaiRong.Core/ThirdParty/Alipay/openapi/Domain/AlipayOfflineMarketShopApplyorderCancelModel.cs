using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayOfflineMarketShopApplyorderCancelModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayOfflineMarketShopApplyorderCancelModel : AopObject
    {
        /// <summary>
        /// 撤销申请流水的原因
        /// </summary>
        [XmlElement("memo")]
        public string Memo { get; set; }

        /// <summary>
        /// 操作人ID，开店请求时候的操作人ID
        /// </summary>
        [XmlElement("op_id")]
        public string OpId { get; set; }

        /// <summary>
        /// 要撤销的订单ID，当店铺创建、修改接口迁移至2.0时，同步返回的apply_id可以用作此接口的入参。
        /// </summary>
        [XmlElement("order_id")]
        public string OrderId { get; set; }
    }
}

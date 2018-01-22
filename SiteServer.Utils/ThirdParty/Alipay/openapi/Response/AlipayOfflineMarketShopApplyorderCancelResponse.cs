using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayOfflineMarketShopApplyorderCancelResponse.
    /// </summary>
    public class AlipayOfflineMarketShopApplyorderCancelResponse : AopResponse
    {
        /// <summary>
        /// 申请撤销成功接口同步返回的支付宝申请流水号。
        /// </summary>
        [XmlElement("apply_id")]
        public string ApplyId { get; set; }
    }
}

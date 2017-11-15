using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiShopMallAuditQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiShopMallAuditQueryModel : AopObject
    {
        /// <summary>
        /// koubei.shop.mall.page.modify(商圈主页地址创建修改接口)中 返回的工单id
        /// </summary>
        [XmlElement("order_flow_id")]
        public string OrderFlowId { get; set; }
    }
}

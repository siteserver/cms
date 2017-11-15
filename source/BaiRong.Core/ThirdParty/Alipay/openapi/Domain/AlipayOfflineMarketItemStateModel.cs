using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayOfflineMarketItemStateModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayOfflineMarketItemStateModel : AopObject
    {
        /// <summary>
        /// 审核规则。支付宝内部使用，外部商户不需填写此字段。
        /// </summary>
        [XmlElement("audit_rule")]
        public AlipayItemAuditRule AuditRule { get; set; }

        /// <summary>
        /// 口碑体系内部商品的唯一标识
        /// </summary>
        [XmlElement("item_id")]
        public string ItemId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [XmlElement("memo")]
        public string Memo { get; set; }

        /// <summary>
        /// 商户通知地址，口碑发消息给商户通知其是否对商品创建、修改、变更状态成功
        /// </summary>
        [XmlElement("operate_notify_url")]
        public string OperateNotifyUrl { get; set; }

        /// <summary>
        /// 商品操作上下文。支付宝内部使用，外部商户不需填写此字段。
        /// </summary>
        [XmlElement("operation_context")]
        public AlipayItemOperationContext OperationContext { get; set; }

        /// <summary>
        /// 支持英文字母和数字，由开发者自行定义（不允许重复），在商品notify消息中也会带有该参数，以此标明本次notify消息是对哪个请求的回应
        /// </summary>
        [XmlElement("request_id")]
        public string RequestId { get; set; }

        /// <summary>
        /// 商品上下架
        /// </summary>
        [XmlElement("state_type")]
        public string StateType { get; set; }
    }
}

using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ActivityOrderDTO Data Structure.
    /// </summary>
    [Serializable]
    public class ActivityOrderDTO : AopObject
    {
        /// <summary>
        /// 工单中的审核信息
        /// </summary>
        [XmlArray("activity_audit_list")]
        [XmlArrayItem("activity_audit_d_t_o")]
        public List<ActivityAuditDTO> ActivityAuditList { get; set; }

        /// <summary>
        /// INIT:初始化;AUDITING:审核中;REJECT:审核驳回;PASS:审核通过;CANCEL:审核撤销;FAIL:审核失败
        /// </summary>
        [XmlElement("audit_status")]
        public string AuditStatus { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        [XmlElement("order_id")]
        public string OrderId { get; set; }

        /// <summary>
        /// INIT:初始化;DOING:处理中;SUCCESS:成功;FAIL:失败
        /// </summary>
        [XmlElement("order_status")]
        public string OrderStatus { get; set; }

        /// <summary>
        /// CAMPAIGN_CREATE_ORDER:创建工单;CAMPAIGN_ENABLE_ORDER:生效工单;CAMPAIGN_START_ORDER:启动工单;CAMPAIGN_CLOSE_ORDER:关闭工单;CAMPAIGN_FINISH_ORDER:结束工单;CAMPAIGN_DELETE_ORDER:删除工单;CAMPAIGN_MODIFY_ORDER:修改工单
        /// </summary>
        [XmlElement("order_type")]
        public string OrderType { get; set; }
    }
}

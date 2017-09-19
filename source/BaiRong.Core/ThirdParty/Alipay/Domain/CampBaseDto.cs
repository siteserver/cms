using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// CampBaseDto Data Structure.
    /// </summary>
    [Serializable]
    public class CampBaseDto : AopObject
    {
        /// <summary>
        /// 活动工单列表
        /// </summary>
        [XmlArray("activity_orders")]
        [XmlArrayItem("activity_order_d_t_o")]
        public List<ActivityOrderDTO> ActivityOrders { get; set; }

        /// <summary>
        /// 活动审核状态，AUDITING为审核中,REJECT为驳回，不返回为成功
        /// </summary>
        [XmlElement("audit_status")]
        public string AuditStatus { get; set; }

        /// <summary>
        /// 是否自动续期，Y为是，N为否，为空表示否
        /// </summary>
        [XmlElement("auto_delay_flag")]
        public string AutoDelayFlag { get; set; }

        /// <summary>
        /// 截至时间
        /// </summary>
        [XmlElement("end_time")]
        public string EndTime { get; set; }

        /// <summary>
        /// 活动id
        /// </summary>
        [XmlElement("id")]
        public string Id { get; set; }

        /// <summary>
        /// 活动名称
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// 招商状态,GOING招商中，ENDED招商结束，OFFLINE下架
        /// </summary>
        [XmlElement("plan_status")]
        public string PlanStatus { get; set; }

        /// <summary>
        /// 启动时间
        /// </summary>
        [XmlElement("start_time")]
        public string StartTime { get; set; }

        /// <summary>
        /// 活动状态,CREATED:草稿，ENABLED：生效，DISABLED：无效，STARTED：启动，CLOSED：停止，FINISHED：完成
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }

        /// <summary>
        /// 活动类型.DIRECT_SEND:直发奖,CONSUME_SEND:消费送
        /// </summary>
        [XmlElement("type")]
        public string Type { get; set; }
    }
}

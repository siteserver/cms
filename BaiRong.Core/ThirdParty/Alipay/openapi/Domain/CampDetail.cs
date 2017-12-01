using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// CampDetail Data Structure.
    /// </summary>
    [Serializable]
    public class CampDetail : AopObject
    {
        /// <summary>
        /// 活动工单列表
        /// </summary>
        [XmlArray("activity_orders")]
        [XmlArrayItem("activity_order_d_t_o")]
        public List<ActivityOrderDTO> ActivityOrders { get; set; }

        /// <summary>
        /// 活动子状态，如审核中
        /// </summary>
        [XmlElement("audit_status")]
        public string AuditStatus { get; set; }

        /// <summary>
        /// 是否自动续期该活动,Y表示是，N表示否，默认为N
        /// </summary>
        [XmlElement("auto_delay_flag")]
        public string AutoDelayFlag { get; set; }

        /// <summary>
        /// 预算信息
        /// </summary>
        [XmlElement("budget_info")]
        public BudgetInfo BudgetInfo { get; set; }

        /// <summary>
        /// 活动约束信息
        /// </summary>
        [XmlElement("constraint_info")]
        public ConstraintInfo ConstraintInfo { get; set; }

        /// <summary>
        /// 活动描述
        /// </summary>
        [XmlElement("desc")]
        public string Desc { get; set; }

        /// <summary>
        /// 活动结束时间
        /// </summary>
        [XmlElement("end_time")]
        public string EndTime { get; set; }

        /// <summary>
        /// 扩展参数
        /// </summary>
        [XmlElement("ext_info")]
        public string ExtInfo { get; set; }

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
        /// 营销工具
        /// </summary>
        [XmlArray("promo_tools")]
        [XmlArrayItem("promo_tool")]
        public List<PromoTool> PromoTools { get; set; }

        /// <summary>
        /// 投放渠道信息
        /// </summary>
        [XmlArray("publish_channels")]
        [XmlArrayItem("publish_channel")]
        public List<PublishChannel> PublishChannels { get; set; }

        /// <summary>
        /// 招商信息
        /// </summary>
        [XmlElement("recruit_info")]
        public RecruitInfo RecruitInfo { get; set; }

        /// <summary>
        /// 活动开始时间
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

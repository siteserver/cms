using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ActivityAuditDTO Data Structure.
    /// </summary>
    [Serializable]
    public class ActivityAuditDTO : AopObject
    {
        /// <summary>
        /// 审核id
        /// </summary>
        [XmlElement("audit_id")]
        public string AuditId { get; set; }

        /// <summary>
        /// INIT:初始化;AUDITING:审核中;REJECT:审核驳回;PASS:审核通过;CANCEL:审核撤销;FAIL:审核失败
        /// </summary>
        [XmlElement("audit_status")]
        public string AuditStatus { get; set; }

        /// <summary>
        /// 操作人id
        /// </summary>
        [XmlElement("creator_id")]
        public string CreatorId { get; set; }

        /// <summary>
        /// SALES:口碑内部小二;PROVIDER:外部服务商;PROVIDER_STAFF:外部服务商员工
        /// </summary>
        [XmlElement("creator_type")]
        public string CreatorType { get; set; }

        /// <summary>
        /// 操作时间
        /// </summary>
        [XmlElement("operation_time")]
        public string OperationTime { get; set; }

        /// <summary>
        /// 审核通过或者审核驳回的原因
        /// </summary>
        [XmlElement("reason")]
        public string Reason { get; set; }
    }
}

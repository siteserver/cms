using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayItemAuditRule Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayItemAuditRule : AopObject
    {
        /// <summary>
        /// 审核类型，商户授权模式此字段不需要填写。
        /// </summary>
        [XmlElement("audit_type")]
        public string AuditType { get; set; }

        /// <summary>
        /// true：需要审核、false：不需要审核，默认为不需要审核,商户授权模式此字段不需要填写。
        /// </summary>
        [XmlElement("need_audit")]
        public bool NeedAudit { get; set; }
    }
}

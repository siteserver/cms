using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayInsUnderwriteClaimReportQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayInsUnderwriteClaimReportQueryModel : AopObject
    {
        /// <summary>
        /// 理赔申请报案号，通过理赔申请【alipay.ins.scene.claim.apply】接口的返回字段claim_report_no获取
        /// </summary>
        [XmlElement("claim_report_no")]
        public string ClaimReportNo { get; set; }
    }
}

using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEcoMycarMaintainAftersaleSyncModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEcoMycarMaintainAftersaleSyncModel : AopObject
    {
        /// <summary>
        /// 车主平台售后编号
        /// </summary>
        [XmlElement("aftersale_no")]
        public string AftersaleNo { get; set; }

        /// <summary>
        /// 客服拒绝退款原因描述
        /// </summary>
        [XmlElement("refuse_reason")]
        public string RefuseReason { get; set; }

        /// <summary>
        /// 1：受理 2：拒绝
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }
    }
}

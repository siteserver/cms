using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEcoCplifeUseridentityStatusUpdateModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEcoCplifeUseridentityStatusUpdateModel : AopObject
    {
        /// <summary>
        /// 业务明细
        /// </summary>
        [XmlElement("biz_details")]
        public string BizDetails { get; set; }

        /// <summary>
        /// 当前业务状态
        /// </summary>
        [XmlElement("biz_state")]
        public string BizState { get; set; }

        /// <summary>
        /// 业务单据ID
        /// </summary>
        [XmlElement("req_id")]
        public string ReqId { get; set; }
    }
}

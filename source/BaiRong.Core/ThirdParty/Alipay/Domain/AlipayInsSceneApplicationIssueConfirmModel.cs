using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayInsSceneApplicationIssueConfirmModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayInsSceneApplicationIssueConfirmModel : AopObject
    {
        /// <summary>
        /// 投保订单号
        /// </summary>
        [XmlElement("application_no")]
        public string ApplicationNo { get; set; }
    }
}

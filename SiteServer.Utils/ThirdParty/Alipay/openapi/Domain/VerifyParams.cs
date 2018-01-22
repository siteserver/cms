using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// VerifyParams Data Structure.
    /// </summary>
    [Serializable]
    public class VerifyParams : AopObject
    {
        /// <summary>
        /// 用户证件号后4位
        /// </summary>
        [XmlElement("cert_no")]
        public string CertNo { get; set; }
    }
}

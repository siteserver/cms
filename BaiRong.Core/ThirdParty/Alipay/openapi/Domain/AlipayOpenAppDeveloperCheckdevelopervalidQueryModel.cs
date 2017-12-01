using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayOpenAppDeveloperCheckdevelopervalidQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayOpenAppDeveloperCheckdevelopervalidQueryModel : AopObject
    {
        /// <summary>
        /// 支付宝账号
        /// </summary>
        [XmlElement("logon_id")]
        public string LogonId { get; set; }
    }
}

using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayUserFinanceinfoShareModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayUserFinanceinfoShareModel : AopObject
    {
        /// <summary>
        /// 支付宝会员的userId
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}

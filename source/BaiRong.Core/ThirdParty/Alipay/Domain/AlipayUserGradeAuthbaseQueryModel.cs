using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayUserGradeAuthbaseQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayUserGradeAuthbaseQueryModel : AopObject
    {
        /// <summary>
        /// 用户的支付宝账户ID
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}

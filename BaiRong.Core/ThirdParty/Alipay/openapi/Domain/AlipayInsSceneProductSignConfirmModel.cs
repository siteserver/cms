using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayInsSceneProductSignConfirmModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayInsSceneProductSignConfirmModel : AopObject
    {
        /// <summary>
        /// 产品编码
        /// </summary>
        [XmlElement("prod_code")]
        public string ProdCode { get; set; }

        /// <summary>
        /// 支付宝会员ID
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}

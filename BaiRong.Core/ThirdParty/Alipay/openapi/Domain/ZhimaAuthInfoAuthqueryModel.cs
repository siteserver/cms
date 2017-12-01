using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ZhimaAuthInfoAuthqueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class ZhimaAuthInfoAuthqueryModel : AopObject
    {
        /// <summary>
        /// json格式的内容,包含userId,userId为支付宝用户id,用户授权后商户可以拿到这个支付宝userId
        /// </summary>
        [XmlElement("identity_param")]
        public string IdentityParam { get; set; }
    }
}

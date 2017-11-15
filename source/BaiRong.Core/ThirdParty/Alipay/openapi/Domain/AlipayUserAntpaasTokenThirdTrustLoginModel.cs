using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayUserAntpaasTokenThirdTrustLoginModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayUserAntpaasTokenThirdTrustLoginModel : AopObject
    {
        /// <summary>
        /// 登录的目标业务，目前已经分配的有autoins，代表车险业务
        /// </summary>
        [XmlElement("login_target")]
        public string LoginTarget { get; set; }
    }
}

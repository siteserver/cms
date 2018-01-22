using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayOpenAuthTokenAppModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayOpenAuthTokenAppModel : AopObject
    {
        /// <summary>
        /// 授权码，如果grant_type的值为authorization_code。该值必须填写
        /// </summary>
        [XmlElement("code")]
        public string Code { get; set; }

        /// <summary>
        /// authorization_code表示换取app_auth_token。  refresh_token表示刷新app_auth_token。
        /// </summary>
        [XmlElement("grant_type")]
        public string GrantType { get; set; }

        /// <summary>
        /// 刷新令牌，如果grant_type值为refresh_token。该值不能为空。该值来源于此接口的返回值app_refresh_token（至少需要通过grant_type=authorization_code调用此接口一次才能获取）
        /// </summary>
        [XmlElement("refresh_token")]
        public string RefreshToken { get; set; }
    }
}

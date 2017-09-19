using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayOpenPublicLifeCreateResponse.
    /// </summary>
    public class AlipayOpenPublicLifeCreateResponse : AopResponse
    {
        /// <summary>
        /// 过期时间，在过期时间之前需要拿refresh_token重新换取新的token使用
        /// </summary>
        [XmlElement("expire_time")]
        public string ExpireTime { get; set; }

        /// <summary>
        /// 生活号id，用于表示此生活号唯一性
        /// </summary>
        [XmlElement("public_id")]
        public string PublicId { get; set; }

        /// <summary>
        /// 生活号长链接
        /// </summary>
        [XmlElement("public_long_link")]
        public string PublicLongLink { get; set; }

        /// <summary>
        /// 生活号短链接
        /// </summary>
        [XmlElement("public_short_link")]
        public string PublicShortLink { get; set; }

        /// <summary>
        /// 刷新令牌，当返回值中过期时间快到了，可以通过此令牌来重新刷新令牌，时间自动延长一年
        /// </summary>
        [XmlElement("refresh_token")]
        public string RefreshToken { get; set; }

        /// <summary>
        /// 授权token，通过授权token可以代替已生成的生活号id进行代理操作，例如发送广播等
        /// </summary>
        [XmlElement("token")]
        public string Token { get; set; }
    }
}

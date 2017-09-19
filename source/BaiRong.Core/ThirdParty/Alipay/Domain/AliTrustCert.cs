using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AliTrustCert Data Structure.
    /// </summary>
    [Serializable]
    public class AliTrustCert : AopObject
    {
        /// <summary>
        /// 点击信用认证图标之后的跳转链接
        /// </summary>
        [XmlElement("forward_url")]
        public string ForwardUrl { get; set; }

        /// <summary>
        /// 通过信用认证的图标链接
        /// </summary>
        [XmlElement("icon_url")]
        public string IconUrl { get; set; }

        /// <summary>
        /// 当通过信用认证时,取值为"T";否则为"F".
        /// </summary>
        [XmlElement("is_certified")]
        public string IsCertified { get; set; }

        /// <summary>
        /// 芝麻认证等级，取值为1,2,3
        /// </summary>
        [XmlElement("level")]
        public string Level { get; set; }

        /// <summary>
        /// 当用户未通过芝麻认证时给出的原因提示
        /// </summary>
        [XmlElement("message")]
        public string Message { get; set; }
    }
}

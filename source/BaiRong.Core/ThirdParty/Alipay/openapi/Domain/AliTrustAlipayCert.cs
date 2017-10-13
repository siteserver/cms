using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AliTrustAlipayCert Data Structure.
    /// </summary>
    [Serializable]
    public class AliTrustAlipayCert : AopObject
    {
        /// <summary>
        /// 用户出生日期
        /// </summary>
        [XmlElement("birthday")]
        public string Birthday { get; set; }

        /// <summary>
        /// 点击支付宝实名认证图标之后的跳转链接
        /// </summary>
        [XmlElement("forward_url")]
        public string ForwardUrl { get; set; }

        /// <summary>
        /// 用户性别(M/F)
        /// </summary>
        [XmlElement("gender")]
        public string Gender { get; set; }

        /// <summary>
        /// 支付宝实名认证图标的链接地址
        /// </summary>
        [XmlElement("icon_url")]
        public string IconUrl { get; set; }

        /// <summary>
        /// 当账户为支付宝实名认证时,取值为"T";否则为"F".
        /// </summary>
        [XmlElement("is_certified")]
        public string IsCertified { get; set; }

        /// <summary>
        /// 用户姓名
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }
    }
}

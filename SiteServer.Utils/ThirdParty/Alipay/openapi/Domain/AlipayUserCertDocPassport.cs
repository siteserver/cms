using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayUserCertDocPassport Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayUserCertDocPassport : AopObject
    {
        /// <summary>
        /// base64编码后的主页照片
        /// </summary>
        [XmlElement("encoded_img")]
        public string EncodedImg { get; set; }

        /// <summary>
        /// 失效日期
        /// </summary>
        [XmlElement("expire_date")]
        public string ExpireDate { get; set; }

        /// <summary>
        /// 姓氏，拼音
        /// </summary>
        [XmlElement("family_name")]
        public string FamilyName { get; set; }

        /// <summary>
        /// 名，拼音
        /// </summary>
        [XmlElement("given_name")]
        public string GivenName { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        [XmlElement("number")]
        public string Number { get; set; }
    }
}

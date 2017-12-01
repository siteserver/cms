using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayUserCertDocIDCard Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayUserCertDocIDCard : AopObject
    {
        /// <summary>
        /// 身份证国徽页照片BASE64编码
        /// </summary>
        [XmlElement("encoded_img_emblem")]
        public string EncodedImgEmblem { get; set; }

        /// <summary>
        /// 头像页照片BASE64编码
        /// </summary>
        [XmlElement("encoded_img_identity")]
        public string EncodedImgIdentity { get; set; }

        /// <summary>
        /// 有效期至
        /// </summary>
        [XmlElement("expire_date")]
        public string ExpireDate { get; set; }

        /// <summary>
        /// 身份证姓名
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        [XmlElement("number")]
        public string Number { get; set; }
    }
}

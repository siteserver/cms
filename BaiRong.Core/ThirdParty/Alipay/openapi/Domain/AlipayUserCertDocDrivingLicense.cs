using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayUserCertDocDrivingLicense Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayUserCertDocDrivingLicense : AopObject
    {
        /// <summary>
        /// 准驾车型
        /// </summary>
        [XmlElement("clazz")]
        public string Clazz { get; set; }

        /// <summary>
        /// 证号
        /// </summary>
        [XmlElement("driving_license_no")]
        public string DrivingLicenseNo { get; set; }

        /// <summary>
        /// base64后的主页照片
        /// </summary>
        [XmlElement("encoded_img_main")]
        public string EncodedImgMain { get; set; }

        /// <summary>
        /// base64编码后的副页图片
        /// </summary>
        [XmlElement("encoded_img_vice")]
        public string EncodedImgVice { get; set; }

        /// <summary>
        /// 失效日期
        /// </summary>
        [XmlElement("expire_date")]
        public string ExpireDate { get; set; }

        /// <summary>
        /// 档案编号
        /// </summary>
        [XmlElement("file_no")]
        public string FileNo { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// 生效日期
        /// </summary>
        [XmlElement("valide_date")]
        public string ValideDate { get; set; }
    }
}

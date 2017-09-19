using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// LegalRepresentativeInfo Data Structure.
    /// </summary>
    [Serializable]
    public class LegalRepresentativeInfo : AopObject
    {
        /// <summary>
        /// 法人证件有效期，YYYY-MM-DD格式
        /// </summary>
        [XmlElement("legal_representative_cert_indate")]
        public string LegalRepresentativeCertIndate { get; set; }

        /// <summary>
        /// 法人证件号码
        /// </summary>
        [XmlElement("legal_representative_cert_no")]
        public string LegalRepresentativeCertNo { get; set; }

        /// <summary>
        /// 法人证件背面照片（如证件为身份证则上传身份证国徽面图片）
        /// </summary>
        [XmlElement("legal_representative_cert_pic_back")]
        public string LegalRepresentativeCertPicBack { get; set; }

        /// <summary>
        /// 法人证件正面照片（如证件为身份证则上传身份证头像面图片）
        /// </summary>
        [XmlElement("legal_representative_cert_pic_front")]
        public string LegalRepresentativeCertPicFront { get; set; }

        /// <summary>
        /// 法人证件类型，支持传入的类型为：RESIDENT(居民身份证)括号中为每种类型的释义，不需要将括号中的内容当参数内容传入。
        /// </summary>
        [XmlElement("legal_representative_cert_type")]
        public string LegalRepresentativeCertType { get; set; }

        /// <summary>
        /// 法人姓名
        /// </summary>
        [XmlElement("legal_representative_name")]
        public string LegalRepresentativeName { get; set; }
    }
}

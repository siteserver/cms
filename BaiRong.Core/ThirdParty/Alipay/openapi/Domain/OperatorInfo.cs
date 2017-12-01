using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// OperatorInfo Data Structure.
    /// </summary>
    [Serializable]
    public class OperatorInfo : AopObject
    {
        /// <summary>
        /// 个体工商户经营者证件到期日，格式为YYYY-MM-DD
        /// </summary>
        [XmlElement("operator_cert_indate")]
        public string OperatorCertIndate { get; set; }

        /// <summary>
        /// 个体工商户经营者证件号码
        /// </summary>
        [XmlElement("operator_cert_no")]
        public string OperatorCertNo { get; set; }

        /// <summary>
        /// 个体工商户经营者证件照片背面图片（如证件类型为身份证则上传国徽面图片）
        /// </summary>
        [XmlElement("operator_cert_pic_back")]
        public string OperatorCertPicBack { get; set; }

        /// <summary>
        /// 个体工商户经营者证件正面照片（如证件类型为身份证则需要上传头像面图片）
        /// </summary>
        [XmlElement("operator_cert_pic_front")]
        public string OperatorCertPicFront { get; set; }

        /// <summary>
        /// 个体工商户经营者证件类型，支持传入的类型为：RESIDENT(居民身份证)括号中为每种类型的释义，不需要将括号中的内容当参数内容传入。
        /// </summary>
        [XmlElement("operator_cert_type")]
        public string OperatorCertType { get; set; }

        /// <summary>
        /// 张三
        /// </summary>
        [XmlElement("operator_name")]
        public string OperatorName { get; set; }
    }
}

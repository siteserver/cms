using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipaySecurityRiskDirectionalIpprofileQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipaySecurityRiskDirectionalIpprofileQueryModel : AopObject
    {
        /// <summary>
        /// 身份证号码，非必填参数，用于查询"身份证持有人使用当前IP的概率"属性
        /// </summary>
        [XmlElement("cert_no")]
        public string CertNo { get; set; }

        /// <summary>
        /// IP地址，IP检测服务接口主键，必填
        /// </summary>
        [XmlElement("ip_address")]
        public string IpAddress { get; set; }

        /// <summary>
        /// 手机号码，非必填参数,用于用户更多维度的识别，如"手机号持有人使用当前IP的概率"属性等
        /// </summary>
        [XmlElement("phone")]
        public string Phone { get; set; }
    }
}

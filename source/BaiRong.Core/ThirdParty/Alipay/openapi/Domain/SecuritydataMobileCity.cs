using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// SecuritydataMobileCity Data Structure.
    /// </summary>
    [Serializable]
    public class SecuritydataMobileCity : AopObject
    {
        /// <summary>
        /// 城市
        /// </summary>
        [XmlElement("city")]
        public string City { get; set; }

        /// <summary>
        /// 手机号前七位
        /// </summary>
        [XmlElement("phone_first_7_digits")]
        public string PhoneFirst7Digits { get; set; }

        /// <summary>
        /// 省份
        /// </summary>
        [XmlElement("province")]
        public string Province { get; set; }
    }
}

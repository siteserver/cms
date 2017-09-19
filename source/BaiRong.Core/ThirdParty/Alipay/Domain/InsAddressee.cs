using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// InsAddressee Data Structure.
    /// </summary>
    [Serializable]
    public class InsAddressee : AopObject
    {
        /// <summary>
        /// 收件人详细地址
        /// </summary>
        [XmlElement("address")]
        public string Address { get; set; }

        /// <summary>
        /// 区域编码
        /// </summary>
        [XmlElement("address_code")]
        public string AddressCode { get; set; }

        /// <summary>
        /// 联系地址-城区
        /// </summary>
        [XmlElement("area")]
        public string Area { get; set; }

        /// <summary>
        /// 联系地址-城市
        /// </summary>
        [XmlElement("city")]
        public string City { get; set; }

        /// <summary>
        /// 联系方式(mobile登录号)
        /// </summary>
        [XmlElement("mobile")]
        public string Mobile { get; set; }

        /// <summary>
        /// 收件人姓名
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// 联系地址-电话
        /// </summary>
        [XmlElement("phone")]
        public string Phone { get; set; }

        /// <summary>
        /// 联系地址-省份
        /// </summary>
        [XmlElement("province")]
        public string Province { get; set; }

        /// <summary>
        /// 联系地址-邮编
        /// </summary>
        [XmlElement("zip")]
        public string Zip { get; set; }
    }
}

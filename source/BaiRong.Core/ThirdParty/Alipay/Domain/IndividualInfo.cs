using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// IndividualInfo Data Structure.
    /// </summary>
    [Serializable]
    public class IndividualInfo : AopObject
    {
        /// <summary>
        /// 生日
        /// </summary>
        [XmlElement("date_of_birth")]
        public string DateOfBirth { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        [XmlElement("id_number")]
        public string IdNumber { get; set; }

        /// <summary>
        /// 个人名字
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// 国籍
        /// </summary>
        [XmlElement("nationality")]
        public string Nationality { get; set; }

        /// <summary>
        /// 个人居住地
        /// </summary>
        [XmlElement("residential_address")]
        public string ResidentialAddress { get; set; }

        /// <summary>
        /// 该个体的类型
        /// </summary>
        [XmlElement("type")]
        public string Type { get; set; }
    }
}

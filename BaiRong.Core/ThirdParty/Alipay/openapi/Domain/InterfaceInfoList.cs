using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// InterfaceInfoList Data Structure.
    /// </summary>
    [Serializable]
    public class InterfaceInfoList : AopObject
    {
        /// <summary>
        /// 传入参数固定值:alipay.eco.mycar.parking.userpage.query
        /// </summary>
        [XmlElement("interface_name")]
        public string InterfaceName { get; set; }

        /// <summary>
        /// 传入参数固定值:interface_page
        /// </summary>
        [XmlElement("interface_type")]
        public string InterfaceType { get; set; }

        /// <summary>
        /// SPI接口的调用地址url，协议必须为https，对整个url字符串必须进行UrlEncode编码。编码为UTF-8
        /// </summary>
        [XmlElement("interface_url")]
        public string InterfaceUrl { get; set; }
    }
}

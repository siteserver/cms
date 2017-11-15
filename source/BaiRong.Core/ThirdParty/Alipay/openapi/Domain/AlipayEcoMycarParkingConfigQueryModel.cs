using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEcoMycarParkingConfigQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEcoMycarParkingConfigQueryModel : AopObject
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
    }
}

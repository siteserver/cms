using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// SsdataDataserviceRiskDigitalidentityQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class SsdataDataserviceRiskDigitalidentityQueryModel : AopObject
    {
        /// <summary>
        /// 服务端生成的设备码（由我方提供的sdk生成）
        /// </summary>
        [XmlElement("device_code")]
        public string DeviceCode { get; set; }
    }
}

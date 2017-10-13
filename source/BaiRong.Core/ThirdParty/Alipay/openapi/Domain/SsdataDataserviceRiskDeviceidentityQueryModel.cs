using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// SsdataDataserviceRiskDeviceidentityQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class SsdataDataserviceRiskDeviceidentityQueryModel : AopObject
    {
        /// <summary>
        /// 设备token值， 服务端JS生成的设备token值（由我方提供的JS生成），界面引入我方提供的js即可获取afs_token值，token的失效期为12个小时，基本上是190位的一串字符。
        /// </summary>
        [XmlElement("device_token")]
        public string DeviceToken { get; set; }
    }
}

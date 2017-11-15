using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// BeaconDeviceInfo Data Structure.
    /// </summary>
    [Serializable]
    public class BeaconDeviceInfo : AopObject
    {
        /// <summary>
        /// 设备类型
        /// </summary>
        [XmlElement("actiontype")]
        public string Actiontype { get; set; }

        /// <summary>
        /// 设备是否可用
        /// </summary>
        [XmlElement("inuse")]
        public bool Inuse { get; set; }

        /// <summary>
        /// 设备说明
        /// </summary>
        [XmlElement("remark")]
        public string Remark { get; set; }

        /// <summary>
        /// 设备序列号
        /// </summary>
        [XmlElement("sn")]
        public string Sn { get; set; }

        /// <summary>
        /// 蓝牙设备关联的模板信息
        /// </summary>
        [XmlElement("template")]
        public BeaconTemplate Template { get; set; }

        /// <summary>
        /// 设备ID
        /// </summary>
        [XmlElement("uuid")]
        public string Uuid { get; set; }
    }
}

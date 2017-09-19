using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayUserDeviceInfo Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayUserDeviceInfo : AopObject
    {
        /// <summary>
        /// 扩展信息，json格式的字符串
        /// </summary>
        [XmlElement("ext_info")]
        public string ExtInfo { get; set; }

        /// <summary>
        /// 移动设备国际身份码缩写：移动设备国际身份码缩写。仅移动端
        /// </summary>
        [XmlElement("imei")]
        public string Imei { get; set; }

        /// <summary>
        /// ipv4地址
        /// </summary>
        [XmlElement("ip")]
        public string Ip { get; set; }

        /// <summary>
        /// mac地址，冒号分隔
        /// </summary>
        [XmlElement("mac")]
        public string Mac { get; set; }

        /// <summary>
        /// 操作系统名称
        /// </summary>
        [XmlElement("os_name")]
        public string OsName { get; set; }

        /// <summary>
        /// 操作系统版本号
        /// </summary>
        [XmlElement("os_version")]
        public string OsVersion { get; set; }
    }
}

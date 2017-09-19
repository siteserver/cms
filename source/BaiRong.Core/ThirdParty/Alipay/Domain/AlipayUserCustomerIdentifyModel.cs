using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayUserCustomerIdentifyModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayUserCustomerIdentifyModel : AopObject
    {
        /// <summary>
        /// 预留参数，用于商户区分同一appId下的不同业务场景。默认场景不用传。
        /// </summary>
        [XmlElement("biz_type")]
        public string BizType { get; set; }

        /// <summary>
        /// 设备及环境信息
        /// </summary>
        [XmlElement("device_info")]
        public AlipayUserDeviceInfo DeviceInfo { get; set; }

        /// <summary>
        /// 预留业务扩展信息
        /// </summary>
        [XmlElement("ext_info")]
        public string ExtInfo { get; set; }

        /// <summary>
        /// 用户主体信息。要求AlipayUserPrincipalInfo中的user_id、mobile、email属性，有且只有一个非空。否则接口会忽略除去优先级最高的属性之外的其他属性。
        /// </summary>
        [XmlElement("principal")]
        public AlipayUserPrincipalInfo Principal { get; set; }
    }
}

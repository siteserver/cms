using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// MdCodeInfoDTO Data Structure.
    /// </summary>
    [Serializable]
    public class MdCodeInfoDTO : AopObject
    {
        /// <summary>
        /// 本次回传动态码的状态：  SUCCESS: 本次发码成功  FAIL_RETRY: 本次发码失败，且需要支付宝重试（重新通知商户发码）  FAIL_NOT_RETRY: 本次发码失败，且无需支付宝重试（无需重新通知商户发码）
        /// </summary>
        [XmlElement("code_status")]
        public string CodeStatus { get; set; }

        /// <summary>
        /// 动态码的码值：  code_status为SUCCESS时必填；  基于此码值生成条形码或二维码用于扫码核销。
        /// </summary>
        [XmlElement("code_value")]
        public string CodeValue { get; set; }

        /// <summary>
        /// 当前动态码的过期（失效）时间：   code_status为SUCCESS时必填。
        /// </summary>
        [XmlElement("expire_time")]
        public string ExpireTime { get; set; }

        /// <summary>
        /// 商户回传动态码的时间戳。    即商户调接口回传动态码时刻对应的long类型时间戳，用于区分不同的发码请求。
        /// </summary>
        [XmlElement("time_stamp")]
        public long TimeStamp { get; set; }
    }
}

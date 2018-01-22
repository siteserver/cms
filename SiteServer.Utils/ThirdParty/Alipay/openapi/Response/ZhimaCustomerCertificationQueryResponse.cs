using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// ZhimaCustomerCertificationQueryResponse.
    /// </summary>
    public class ZhimaCustomerCertificationQueryResponse : AopResponse
    {
        /// <summary>
        /// 认证过程成认证过的各渠道的状态,中间材料等数据
        /// </summary>
        [XmlElement("channel_statuses")]
        public string ChannelStatuses { get; set; }

        /// <summary>
        /// 如果认证没有通过会显示失败原因,更详细的情况在channel_statuses参数里面
        /// </summary>
        [XmlElement("failed_reason")]
        public string FailedReason { get; set; }

        /// <summary>
        /// 认证是否通过,通过为true,不通过为false
        /// </summary>
        [XmlElement("passed")]
        public string Passed { get; set; }
    }
}

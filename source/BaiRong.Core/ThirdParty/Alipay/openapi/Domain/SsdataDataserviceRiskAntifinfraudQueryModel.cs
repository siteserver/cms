using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// SsdataDataserviceRiskAntifinfraudQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class SsdataDataserviceRiskAntifinfraudQueryModel : AopObject
    {
        /// <summary>
        /// 行为发生时的Apdid值
        /// </summary>
        [XmlElement("apdid")]
        public string Apdid { get; set; }

        /// <summary>
        /// 行为发生时的IP值
        /// </summary>
        [XmlElement("ip")]
        public string Ip { get; set; }

        /// <summary>
        /// 调用服务的商户id ，如果是第三方服务商，需要将其实际的商户id透传过来，如果是普通商户传入自己的appid或不填
        /// </summary>
        [XmlElement("partner_id")]
        public string PartnerId { get; set; }

        /// <summary>
        /// 行为发生时的手机号
        /// </summary>
        [XmlElement("phone")]
        public string Phone { get; set; }

        /// <summary>
        /// 风险场景码
        /// </summary>
        [XmlElement("risk_code")]
        public string RiskCode { get; set; }

        /// <summary>
        /// 业务场景
        /// </summary>
        [XmlElement("scene_code")]
        public string SceneCode { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        [XmlElement("sys_version")]
        public string SysVersion { get; set; }

        /// <summary>
        /// 行为发生时的UMID值
        /// </summary>
        [XmlElement("umid")]
        public string Umid { get; set; }
    }
}

using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// TemplateOpenCardConfDTO Data Structure.
    /// </summary>
    [Serializable]
    public class TemplateOpenCardConfDTO : AopObject
    {
        /// <summary>
        /// 配置，预留字段，暂时不用
        /// </summary>
        [XmlElement("conf")]
        public string Conf { get; set; }

        /// <summary>
        /// ISV：外部系统  MER：直连商户
        /// </summary>
        [XmlElement("open_card_source_type")]
        public string OpenCardSourceType { get; set; }

        /// <summary>
        /// 开卡连接，必须http、https开头
        /// </summary>
        [XmlElement("open_card_url")]
        public string OpenCardUrl { get; set; }

        /// <summary>
        /// 渠道APPID，提供领卡页面的服务提供方
        /// </summary>
        [XmlElement("source_app_id")]
        public string SourceAppId { get; set; }
    }
}

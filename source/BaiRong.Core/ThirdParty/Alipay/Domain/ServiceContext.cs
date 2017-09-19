using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ServiceContext Data Structure.
    /// </summary>
    [Serializable]
    public class ServiceContext : AopObject
    {
        /// <summary>
        /// 客户端IP
        /// </summary>
        [XmlElement("client_ip")]
        public string ClientIp { get; set; }

        /// <summary>
        /// 客户端UMID
        /// </summary>
        [XmlElement("client_pcidguid")]
        public string ClientPcidguid { get; set; }

        /// <summary>
        /// 服务器名
        /// </summary>
        [XmlElement("server_name")]
        public string ServerName { get; set; }

        /// <summary>
        /// 会话ID
        /// </summary>
        [XmlElement("session_id")]
        public string SessionId { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}

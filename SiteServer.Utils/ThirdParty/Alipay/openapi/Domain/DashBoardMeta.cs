using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// DashBoardMeta Data Structure.
    /// </summary>
    [Serializable]
    public class DashBoardMeta : AopObject
    {
        /// <summary>
        /// 授权状态，值为true或者false
        /// </summary>
        [XmlElement("auth_status")]
        public bool AuthStatus { get; set; }

        /// <summary>
        /// 仪表盘创建时间
        /// </summary>
        [XmlElement("create_time")]
        public string CreateTime { get; set; }

        /// <summary>
        /// 仪表盘ID
        /// </summary>
        [XmlElement("dashboard_id")]
        public string DashboardId { get; set; }

        /// <summary>
        /// 仪表盘名称
        /// </summary>
        [XmlElement("dashboard_name")]
        public string DashboardName { get; set; }
    }
}

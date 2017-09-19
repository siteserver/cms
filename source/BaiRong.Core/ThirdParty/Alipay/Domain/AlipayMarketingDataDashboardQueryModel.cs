using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayMarketingDataDashboardQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayMarketingDataDashboardQueryModel : AopObject
    {
        /// <summary>
        /// 仪表盘ID
        /// </summary>
        [XmlElement("dashboard_id")]
        public string DashboardId { get; set; }

        /// <summary>
        /// 仪表盘过滤条件
        /// </summary>
        [XmlArray("param")]
        [XmlArrayItem("dashboard_param")]
        public List<DashboardParam> Param { get; set; }
    }
}

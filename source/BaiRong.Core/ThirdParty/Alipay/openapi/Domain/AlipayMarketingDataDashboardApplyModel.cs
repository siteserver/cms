using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayMarketingDataDashboardApplyModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayMarketingDataDashboardApplyModel : AopObject
    {
        /// <summary>
        /// 仪表盘ID列表
        /// </summary>
        [XmlArray("dashboard_ids")]
        [XmlArrayItem("string")]
        public List<string> DashboardIds { get; set; }
    }
}

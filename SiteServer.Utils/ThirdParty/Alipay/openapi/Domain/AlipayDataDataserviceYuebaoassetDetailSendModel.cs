using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayDataDataserviceYuebaoassetDetailSendModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayDataDataserviceYuebaoassetDetailSendModel : AopObject
    {
        /// <summary>
        /// 资产负债报表数据列表
        /// </summary>
        [XmlArray("alm_report_data")]
        [XmlArrayItem("alm_report_data")]
        public List<AlmReportData> AlmReportData { get; set; }
    }
}

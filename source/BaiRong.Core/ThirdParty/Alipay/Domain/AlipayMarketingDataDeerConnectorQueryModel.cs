using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayMarketingDataDeerConnectorQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayMarketingDataDeerConnectorQueryModel : AopObject
    {
        /// <summary>
        /// 活动洞察数据查询标识
        /// </summary>
        [XmlElement("connector_id")]
        public string ConnectorId { get; set; }

        /// <summary>
        /// 数据请求的参数，比如活动投放日期、投放渠道等信息
        /// </summary>
        [XmlElement("params")]
        public string Params { get; set; }
    }
}

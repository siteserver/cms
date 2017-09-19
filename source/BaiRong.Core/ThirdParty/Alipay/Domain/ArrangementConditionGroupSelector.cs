using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ArrangementConditionGroupSelector Data Structure.
    /// </summary>
    [Serializable]
    public class ArrangementConditionGroupSelector : AopObject
    {
        /// <summary>
        /// 是否选择最新的产品条件,默认为TRUE
        /// </summary>
        [XmlElement("select_latest_pd_cd")]
        public bool SelectLatestPdCd { get; set; }
    }
}

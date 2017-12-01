using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AntProdpaasArrangementCommonQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AntProdpaasArrangementCommonQueryModel : AopObject
    {
        /// <summary>
        /// 合约基本信息选择器，根据产品编码，合约状态编码来过滤合约
        /// </summary>
        [XmlElement("arrangement_base_selector")]
        public ArrangementBaseSelector ArrangementBaseSelector { get; set; }

        /// <summary>
        /// 合约条件组选择器，筛选合约条件
        /// </summary>
        [XmlElement("arrangement_condition_group_selector")]
        public ArrangementConditionGroupSelector ArrangementConditionGroupSelector { get; set; }

        /// <summary>
        /// 合约参与者选择器，根据参与者查询合约编号,与合约号选择器二选一
        /// </summary>
        [XmlElement("arrangement_involved_party_querier")]
        public ArrangementInvolvedPartyQuerier ArrangementInvolvedPartyQuerier { get; set; }

        /// <summary>
        /// 合约号查询器，与参与者查询器二选一
        /// </summary>
        [XmlElement("arrangement_no_querier")]
        public ArrangementNoQuerier ArrangementNoQuerier { get; set; }
    }
}

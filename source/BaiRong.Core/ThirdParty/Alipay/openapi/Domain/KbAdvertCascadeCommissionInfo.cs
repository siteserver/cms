using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KbAdvertCascadeCommissionInfo Data Structure.
    /// </summary>
    [Serializable]
    public class KbAdvertCascadeCommissionInfo : AopObject
    {
        /// <summary>
        /// 二级分佣条款信息
        /// </summary>
        [XmlArray("commission_clause_infos")]
        [XmlArrayItem("kb_advert_commission_clause")]
        public List<KbAdvertCommissionClause> CommissionClauseInfos { get; set; }

        /// <summary>
        /// 二级分佣任务认领人类型  PROMOTER：其他推广者  KOUBEI_PLATFORM：口碑平台
        /// </summary>
        [XmlElement("commission_user_type")]
        public string CommissionUserType { get; set; }
    }
}

using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiAdvertCommissionCascademissionCreateModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiAdvertCommissionCascademissionCreateModel : AopObject
    {
        /// <summary>
        /// 子任务的分佣配置
        /// </summary>
        [XmlArray("cascade_mission_conf")]
        [XmlArrayItem("cascade_mission_conf_model")]
        public List<CascadeMissionConfModel> CascadeMissionConf { get; set; }

        /// <summary>
        /// 根据identify_type指定的值  misison时，为需要设置子任务的分佣任务ID  voucher时，为需要券ID
        /// </summary>
        [XmlElement("identify")]
        public string Identify { get; set; }

        /// <summary>
        /// 主键类型  mission：已经领取的任务，需要在该任务下发布子任务的ID  voucher：任务对应的券ID
        /// </summary>
        [XmlElement("identify_type")]
        public string IdentifyType { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }
    }
}

using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// InsDataAutodamageEstimateConfirmModel Data Structure.
    /// </summary>
    [Serializable]
    public class InsDataAutodamageEstimateConfirmModel : AopObject
    {
        /// <summary>
        /// 受损程度
        /// </summary>
        [XmlElement("damage_degree")]
        public string DamageDegree { get; set; }

        /// <summary>
        /// 受损程度中文描述
        /// </summary>
        [XmlElement("damage_degree_desc")]
        public string DamageDegreeDesc { get; set; }

        /// <summary>
        /// 受损类型
        /// </summary>
        [XmlElement("damage_type")]
        public string DamageType { get; set; }

        /// <summary>
        /// 受损类型中文描述
        /// </summary>
        [XmlElement("damage_type_desc")]
        public string DamageTypeDesc { get; set; }

        /// <summary>
        /// 工时费，单位：元
        /// </summary>
        [XmlElement("hourly_wage")]
        public string HourlyWage { get; set; }

        /// <summary>
        /// 是否旧件回收
        /// </summary>
        [XmlElement("old_recycle")]
        public bool OldRecycle { get; set; }

        /// <summary>
        /// 配件费用，单位：元
        /// </summary>
        [XmlElement("parts_cost")]
        public string PartsCost { get; set; }

        /// <summary>
        /// 配件id
        /// </summary>
        [XmlElement("parts_id")]
        public string PartsId { get; set; }

        /// <summary>
        /// 零件管理费，单位：元
        /// </summary>
        [XmlElement("parts_manage_fee")]
        public string PartsManageFee { get; set; }

        /// <summary>
        /// 配件名称
        /// </summary>
        [XmlElement("parts_name")]
        public string PartsName { get; set; }

        /// <summary>
        /// 残值扣除，单位：元
        /// </summary>
        [XmlElement("remain_value")]
        public string RemainValue { get; set; }

        /// <summary>
        /// 维修方案
        /// </summary>
        [XmlElement("repair_type")]
        public string RepairType { get; set; }

        /// <summary>
        /// 维修方案中文描述
        /// </summary>
        [XmlElement("repair_type_desc")]
        public string RepairTypeDesc { get; set; }
    }
}

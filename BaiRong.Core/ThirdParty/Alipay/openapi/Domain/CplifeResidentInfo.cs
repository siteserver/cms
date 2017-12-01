using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// CplifeResidentInfo Data Structure.
    /// </summary>
    [Serializable]
    public class CplifeResidentInfo : AopObject
    {
        /// <summary>
        /// 业主关联的房产在支付宝平台中的唯一标识。(该属性和property_entity_id两个至少需要填写一项，如果两项都填写则以entity_id为准.)
        /// </summary>
        [XmlElement("entity_id")]
        public string EntityId { get; set; }

        /// <summary>
        /// 业主身份证号的MD5结果
        /// </summary>
        [XmlElement("idcard_no")]
        public string IdcardNo { get; set; }

        /// <summary>
        /// 用户的真实姓名
        /// </summary>
        [XmlElement("name")]
        public string Name { get; set; }

        /// <summary>
        /// 业主关联的房产在物业系统中的唯一标识。(该属性和entity_id两个至少需要填写一项，如果两项都填写则以entity_id为准.)
        /// </summary>
        [XmlElement("out_entity_id")]
        public string OutEntityId { get; set; }

        /// <summary>
        /// 物业系统中小区住户的唯一ID标识.
        /// </summary>
        [XmlElement("out_resident_id")]
        public string OutResidentId { get; set; }

        /// <summary>
        /// 用户对房产的关系类型。业主:1；家人:2；租客:3
        /// </summary>
        [XmlElement("type")]
        public string Type { get; set; }
    }
}

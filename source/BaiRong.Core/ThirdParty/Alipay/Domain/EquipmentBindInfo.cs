using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// EquipmentBindInfo Data Structure.
    /// </summary>
    [Serializable]
    public class EquipmentBindInfo : AopObject
    {
        /// <summary>
        /// 机具ID
        /// </summary>
        [XmlElement("equipment_id")]
        public string EquipmentId { get; set; }

        /// <summary>
        /// 是否绑定门店，T绑定，F不绑定
        /// </summary>
        [XmlElement("is_bind_shop")]
        public string IsBindShop { get; set; }
    }
}

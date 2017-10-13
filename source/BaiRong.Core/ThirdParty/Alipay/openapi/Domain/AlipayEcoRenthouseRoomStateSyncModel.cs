using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEcoRenthouseRoomStateSyncModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEcoRenthouseRoomStateSyncModel : AopObject
    {
        /// <summary>
        /// 房源类型（1:分散式 2：集中式）
        /// </summary>
        [XmlElement("flats_tag")]
        public long FlatsTag { get; set; }

        /// <summary>
        /// 出租状态（1未租、2已租）
        /// </summary>
        [XmlElement("rent_status")]
        public long RentStatus { get; set; }

        /// <summary>
        /// 公寓运营商内部存储的房源编号(ka系统的房源id)
        /// </summary>
        [XmlElement("room_code")]
        public string RoomCode { get; set; }

        /// <summary>
        /// 是否上架，0:下架，1：上架
        /// </summary>
        [XmlElement("room_status")]
        public long RoomStatus { get; set; }
    }
}

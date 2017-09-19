using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// CplifeRoomInfoResp Data Structure.
    /// </summary>
    [Serializable]
    public class CplifeRoomInfoResp : AopObject
    {
        /// <summary>
        /// 商户系统小区房屋唯一ID标示.
        /// </summary>
        [XmlElement("out_room_id")]
        public string OutRoomId { get; set; }

        /// <summary>
        /// 支付宝系统房间唯一标示.
        /// </summary>
        [XmlElement("room_id")]
        public string RoomId { get; set; }
    }
}

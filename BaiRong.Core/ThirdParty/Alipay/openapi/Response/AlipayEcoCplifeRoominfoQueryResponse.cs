using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayEcoCplifeRoominfoQueryResponse.
    /// </summary>
    public class AlipayEcoCplifeRoominfoQueryResponse : AopResponse
    {
        /// <summary>
        /// 符合条件的小区房屋信息列表.
        /// </summary>
        [XmlArray("room_info")]
        [XmlArrayItem("cplife_room_detail")]
        public List<CplifeRoomDetail> RoomInfo { get; set; }

        /// <summary>
        /// 该小区下已上传的房间总数
        /// </summary>
        [XmlElement("total_room_number")]
        public long TotalRoomNumber { get; set; }
    }
}

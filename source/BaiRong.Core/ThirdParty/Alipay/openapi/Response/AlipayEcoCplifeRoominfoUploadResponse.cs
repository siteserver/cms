using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayEcoCplifeRoominfoUploadResponse.
    /// </summary>
    public class AlipayEcoCplifeRoominfoUploadResponse : AopResponse
    {
        /// <summary>
        /// 业主所在物业小区ID(支付宝平台唯一小区ID标示)
        /// </summary>
        [XmlElement("community_id")]
        public string CommunityId { get; set; }

        /// <summary>
        /// 已经成功上传的房屋信息列表.
        /// </summary>
        [XmlArray("room_info_set")]
        [XmlArrayItem("cplife_room_info_resp")]
        public List<CplifeRoomInfoResp> RoomInfoSet { get; set; }
    }
}

using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayOpenServicemarketCommodityExtendinfosAddModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayOpenServicemarketCommodityExtendinfosAddModel : AopObject
    {
        /// <summary>
        /// 公服扩展信息列表
        /// </summary>
        [XmlArray("commodity_ext_infos")]
        [XmlArrayItem("commodity_public_ext_infos")]
        public List<CommodityPublicExtInfos> CommodityExtInfos { get; set; }

        /// <summary>
        /// 服务插件ID
        /// </summary>
        [XmlElement("commodity_id")]
        public string CommodityId { get; set; }

        /// <summary>
        /// 应用ID
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}

using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayOpenServicemarketCommodityExtendinfosConfirmModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayOpenServicemarketCommodityExtendinfosConfirmModel : AopObject
    {
        /// <summary>
        /// 公服BD审核扩展信息
        /// </summary>
        [XmlArray("commodity_ext_infos")]
        [XmlArrayItem("commodity_ext_info_confirm")]
        public List<CommodityExtInfoConfirm> CommodityExtInfos { get; set; }

        /// <summary>
        /// 服务Id
        /// </summary>
        [XmlElement("commodity_id")]
        public string CommodityId { get; set; }

        /// <summary>
        /// status 为驳回时 必须输入驳回原因
        /// </summary>
        [XmlElement("memo")]
        public string Memo { get; set; }

        /// <summary>
        /// 公服BD审核结果：成功还是失败:  status 【0：表示不通过 ， 1：表示通过】
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}

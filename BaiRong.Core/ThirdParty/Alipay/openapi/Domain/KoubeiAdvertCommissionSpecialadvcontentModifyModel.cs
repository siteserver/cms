using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiAdvertCommissionSpecialadvcontentModifyModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiAdvertCommissionSpecialadvcontentModifyModel : AopObject
    {
        /// <summary>
        /// 广告ID
        /// </summary>
        [XmlElement("adv_id")]
        public string AdvId { get; set; }

        /// <summary>
        /// 渠道ID（如果修改的是广告的默认主推广的内容，则不传渠道ID；如果修改的是广告的指定投放渠道的内容，则传指定渠道的ID）
        /// </summary>
        [XmlElement("channel_id")]
        public string ChannelId { get; set; }

        /// <summary>
        /// 创建或者删除广告内容的请求参数List
        /// </summary>
        [XmlArray("content_list")]
        [XmlArrayItem("kb_advert_special_adv_content_request")]
        public List<KbAdvertSpecialAdvContentRequest> ContentList { get; set; }

        /// <summary>
        /// 特殊广告内容的修改枚举类型：  create：表示创建特殊广告内容  delete：表示删除特殊广告内容
        /// </summary>
        [XmlElement("modify_type")]
        public string ModifyType { get; set; }
    }
}

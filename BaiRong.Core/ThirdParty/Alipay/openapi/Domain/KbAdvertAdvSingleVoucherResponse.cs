using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KbAdvertAdvSingleVoucherResponse Data Structure.
    /// </summary>
    [Serializable]
    public class KbAdvertAdvSingleVoucherResponse : AopObject
    {
        /// <summary>
        /// 广告内容模型
        /// </summary>
        [XmlArray("adv_content_list")]
        [XmlArrayItem("kb_advert_adv_content_response")]
        public List<KbAdvertAdvContentResponse> AdvContentList { get; set; }

        /// <summary>
        /// 广告内容（广告内容请使用新的属性adv_content_list，此属性仍会保留）
        /// </summary>
        [XmlElement("content")]
        public KbAdvertAdvContent Content { get; set; }

        /// <summary>
        /// 券标的
        /// </summary>
        [XmlElement("voucher")]
        public KbAdvertSubjectVoucherResponse Voucher { get; set; }
    }
}

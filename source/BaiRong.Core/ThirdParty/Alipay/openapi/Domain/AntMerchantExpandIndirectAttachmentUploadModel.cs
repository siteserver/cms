using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AntMerchantExpandIndirectAttachmentUploadModel Data Structure.
    /// </summary>
    [Serializable]
    public class AntMerchantExpandIndirectAttachmentUploadModel : AopObject
    {
        /// <summary>
        /// 商户附件信息
        /// </summary>
        [XmlArray("attachment_info")]
        [XmlArrayItem("attachment_info")]
        public List<AttachmentInfo> AttachmentInfo { get; set; }

        /// <summary>
        /// 备注信息
        /// </summary>
        [XmlElement("memo")]
        public string Memo { get; set; }

        /// <summary>
        /// 商户在支付宝入驻成功后，生成的支付宝内全局唯一的商户编号
        /// </summary>
        [XmlElement("sub_merchant_id")]
        public string SubMerchantId { get; set; }
    }
}

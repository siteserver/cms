using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayInsSceneClaimAttachmentConfirmModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayInsSceneClaimAttachmentConfirmModel : AopObject
    {
        /// <summary>
        /// 理赔申请报案号，通过理赔申请【alipay.ins.scene.claim.apply】接口的返回字段claim_report_no获取
        /// </summary>
        [XmlElement("claim_report_no")]
        public string ClaimReportNo { get; set; }

        /// <summary>
        /// 上传的文件名清单列表，即alipay.ins.scene.claim.attachment.upload  接口中的attachment_name  用逗号(,)隔离
        /// </summary>
        [XmlArray("upload_files")]
        [XmlArrayItem("string")]
        public List<string> UploadFiles { get; set; }
    }
}

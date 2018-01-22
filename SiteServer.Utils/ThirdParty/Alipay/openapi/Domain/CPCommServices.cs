using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// CPCommServices Data Structure.
    /// </summary>
    [Serializable]
    public class CPCommServices : AopObject
    {
        /// <summary>
        /// 对于涉及收费类型的服务，返回收款帐号，若开发者没有为当前服务传入过物业收款帐号，则默认为授权物业的签约账号。
        /// </summary>
        [XmlElement("account")]
        public string Account { get; set; }

        /// <summary>
        /// 若当前服务涉及收费，则返回收款帐号类型。
        /// </summary>
        [XmlElement("account_type")]
        public string AccountType { get; set; }

        /// <summary>
        /// 服务审核状态描述，如果审核驳回则有相关的驳回理由。
        /// </summary>
        [XmlElement("audit_desc")]
        public string AuditDesc { get; set; }

        /// <summary>
        /// 服务审核状态。
        /// </summary>
        [XmlElement("audit_status")]
        public string AuditStatus { get; set; }

        /// <summary>
        /// 服务对应的前台类目名称
        /// </summary>
        [XmlElement("category_name")]
        public string CategoryName { get; set; }

        /// <summary>
        /// 该字段可选，若对于外部调用地址巡检失败，会返回失败状态。
        /// </summary>
        [XmlElement("external_address_scan_result")]
        public string ExternalAddressScanResult { get; set; }

        /// <summary>
        /// 由开发者系统提供的，支付宝根据基础服务类型在特定业务环节调用的外部系统服务地址。
        /// </summary>
        [XmlElement("external_invoke_address")]
        public string ExternalInvokeAddress { get; set; }

        /// <summary>
        /// 服务初始化时间
        /// </summary>
        [XmlElement("gmt_created")]
        public string GmtCreated { get; set; }

        /// <summary>
        /// 服务最近修改时间（包括状态变更）。
        /// </summary>
        [XmlElement("gmt_modified")]
        public string GmtModified { get; set; }

        /// <summary>
        /// 若从当前状态到下一状态需要完成下一步条件代码，则返回该字段，否则不返回。
        /// </summary>
        [XmlElement("next_action")]
        public string NextAction { get; set; }

        /// <summary>
        /// 若qr_code_image二维码存在有效期，则返回。
        /// </summary>
        [XmlElement("qr_code_expires")]
        public string QrCodeExpires { get; set; }

        /// <summary>
        /// 为满足特定的服务类型在上线前后的不同阶段需要进行测试验证等目的，选择性返回能直达具体服务的二维码图片链接。用支付宝手机客户端扫一扫该链接，完成验证工作。
        /// </summary>
        [XmlElement("qr_code_image")]
        public string QrCodeImage { get; set; }

        /// <summary>
        /// 若返回qr_code_image，则同时返回对应的类型，类型值为：  TEST - 用于上线前验证的临时二维码；  FORMAL - 上线后可用于推广的正式二维码（仅针对部分服务类型）；
        /// </summary>
        [XmlElement("qr_code_type")]
        public string QrCodeType { get; set; }

        /// <summary>
        /// 本服务预计过期时间（如在物业服务合同中约定），按标准时间格式：yyyy-MM-dd HH:mm:ss返回。
        /// </summary>
        [XmlElement("service_expires")]
        public string ServiceExpires { get; set; }

        /// <summary>
        /// 服务类型
        /// </summary>
        [XmlElement("service_type")]
        public string ServiceType { get; set; }

        /// <summary>
        /// 服务当前状态
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }
    }
}

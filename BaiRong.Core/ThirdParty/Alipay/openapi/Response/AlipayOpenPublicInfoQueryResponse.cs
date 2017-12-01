using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayOpenPublicInfoQueryResponse.
    /// </summary>
    public class AlipayOpenPublicInfoQueryResponse : AopResponse
    {
        /// <summary>
        /// 服务窗名称
        /// </summary>
        [XmlElement("app_name")]
        public string AppName { get; set; }

        /// <summary>
        /// 服务窗审核状态描述，如果审核驳回则有相关的驳回理由
        /// </summary>
        [XmlElement("audit_desc")]
        public string AuditDesc { get; set; }

        /// <summary>
        /// 服务窗审核状态，对于系统商而言，只有四个状态，AUDITING：审核中，AUDIT_FAILED：审核驳回，AUDIT_SUCCESS：审核通过，AUDIT_NORMAL：无审核状态（当前没有处于审核过程的工单）
        /// </summary>
        [XmlElement("audit_status")]
        public string AuditStatus { get; set; }

        /// <summary>
        /// 服务窗是否上线，T表示上线，F表示未上线
        /// </summary>
        [XmlElement("is_online")]
        public string IsOnline { get; set; }

        /// <summary>
        /// 服务窗是否上架，T表示上架，上架即可在支付宝客户端被搜索到，F表示未上架
        /// </summary>
        [XmlElement("is_release")]
        public string IsRelease { get; set; }

        /// <summary>
        /// 服务窗头像地址
        /// </summary>
        [XmlElement("logo_url")]
        public string LogoUrl { get; set; }

        /// <summary>
        /// 服务窗欢迎语
        /// </summary>
        [XmlElement("public_greeting")]
        public string PublicGreeting { get; set; }
    }
}

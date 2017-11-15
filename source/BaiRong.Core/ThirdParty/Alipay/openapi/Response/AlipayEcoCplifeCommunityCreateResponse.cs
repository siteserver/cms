using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayEcoCplifeCommunityCreateResponse.
    /// </summary>
    public class AlipayEcoCplifeCommunityCreateResponse : AopResponse
    {
        /// <summary>
        /// 若小区创建成功，则返回支付宝社区物业平台中的小区统一编号。
        /// </summary>
        [XmlElement("community_id")]
        public string CommunityId { get; set; }

        /// <summary>
        /// 小区创建成功后，若从当前状态到下一状态需要完成下一步条件代码。则返回该字段，否则不返回。  格式为“条件代码|操作主体”。  操作主体有：  INVOKER - 接口调用方  MERCHANT - 物业公司  AUDITOR - 平台审核方  条件代码包括但不限于：  WAIT_SERVICE_PROVISION - 等待基础服务初始化  WAIT_PROD_VERIFICATION - 等待在生产环境通过自测  WAIT_ONLINE_APPLICATION - 等待提起上线申请  WAIT_OFFLINE_APPLICATION - 等待提起下线申请  WAIT_CONFIRMATION - 等待相关方确认  WAIT_AUDITING - 等待审核
        /// </summary>
        [XmlElement("next_action")]
        public string NextAction { get; set; }

        /// <summary>
        /// 若小区创建成功，则返回当前状态，状态值：  PENDING_ONLINE 待上线  ONLINE - 上线  MAINTAIN - 维护中  OFFLINE - 下线    新创建的小区为PENDING_ONLINE待上线状态，需要尽快初始化基础服务，完成开发和验证，并提交服务上线申请。由支付宝小二审核通过后完成服务和小区上线。
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }
    }
}

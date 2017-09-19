using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayEcoCplifeBasicserviceModifyResponse.
    /// </summary>
    public class AlipayEcoCplifeBasicserviceModifyResponse : AopResponse
    {
        /// <summary>
        /// 服务变更成功后，若从当前状态到下一状态需要完成下一步条件代码，则返回该字段，否则不返回。  格式为“条件代码|操作主体”。  操作主体有：  INVOKER - 接口调用方  MERCHANT - 物业公司  AUDITOR - 平台审核方  条件代码包括但不限于：  WAIT_SERVICE_PROVISION - 等待基础服务初始化  WAIT_PROD_VERIFICATION - 等待在生产环境通过自测  WAIT_ONLINE_APPLICATION - 等待提起上线申请（目前支持开发者调用alipay.eco.cplife.basicservice.modify接口发起上线申请）  WAIT_AUTH_TO_PLATFORM - 等待物业授权给支付宝社区平台  WAIT_CONFIRMATION - 等待相关方确认  WAIT_AUDITING - 等待审核
        /// </summary>
        [XmlElement("next_action")]
        public string NextAction { get; set; }

        /// <summary>
        /// 若服务成功变更，则返回当前状态。
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }
    }
}

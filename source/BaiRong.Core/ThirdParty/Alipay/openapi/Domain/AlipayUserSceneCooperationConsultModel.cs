using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayUserSceneCooperationConsultModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayUserSceneCooperationConsultModel : AopObject
    {
        /// <summary>
        /// 用户主体信息。要求AlipayUserPrincipalInfo中的user_id、mobile(不加国家码)、email属性，有且只有一个非空。否则接口会忽略除去优先级最高的属性之外的其他属性。user_id优先级最高，mobile次之，email最后
        /// </summary>
        [XmlElement("principal")]
        public AlipayUserPrincipalInfo Principal { get; set; }

        /// <summary>
        /// 商户的场景定义，需要支付宝对接入场景进行配置。
        /// </summary>
        [XmlElement("scene")]
        public string Scene { get; set; }
    }
}

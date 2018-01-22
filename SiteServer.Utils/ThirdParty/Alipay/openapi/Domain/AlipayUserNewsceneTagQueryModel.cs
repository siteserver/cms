using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayUserNewsceneTagQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayUserNewsceneTagQueryModel : AopObject
    {
        /// <summary>
        /// 用户主体信息。要求AlipayUserPrincipalInfo中的user_id、mobile(不加国家码)、email属性，有且只有一个非空。否则接口会忽略除去优先级最高的属性之外的其他属性。user_id优先级最高，mobile次之，email最后
        /// </summary>
        [XmlElement("principal")]
        public AlipayUserPrincipalInfo Principal { get; set; }

        /// <summary>
        /// 要查询哪些新的标签，多个场景请用,隔开。注意该字段受scene控制，支付宝会给scene分配可以查询的标签，无效的请求会报参数异常
        /// </summary>
        [XmlElement("query_tags")]
        public string QueryTags { get; set; }

        /// <summary>
        /// 调用该接口的场景，由支付宝分配，如果是无效场景，将视为无效访问，并且该场景约束所查新标签的类型，如果不符合则报参数异常
        /// </summary>
        [XmlElement("scene")]
        public string Scene { get; set; }
    }
}

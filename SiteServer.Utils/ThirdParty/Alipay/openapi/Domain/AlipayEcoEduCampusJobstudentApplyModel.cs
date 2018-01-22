using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEcoEduCampusJobstudentApplyModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEcoEduCampusJobstudentApplyModel : AopObject
    {
        /// <summary>
        /// 支付宝客户端用户Id
        /// </summary>
        [XmlElement("alipay_user_id")]
        public string AlipayUserId { get; set; }

        /// <summary>
        /// 备用字段，JSON格式.（可使用单引号或者双引号）
        /// </summary>
        [XmlElement("content_var")]
        public string ContentVar { get; set; }
    }
}

using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiMemberDataOauthQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiMemberDataOauthQueryModel : AopObject
    {
        /// <summary>
        /// 授权业务类型，目前统一只有pay_member
        /// </summary>
        [XmlElement("auth_type")]
        public string AuthType { get; set; }

        /// <summary>
        /// 授权码，用于换取授权信息如操作人id等.获取方式:跳转isv地址中会带有此code参数。auth_code一次有效，auth_code有效期为3分钟到24小时（开放平台规则会根据具体的业务场景动态调整auth_code的有效期，但是不会低于3分钟，同时也不会超过24小时），超过有效期的auth_code即使未使用也将无法使用。用户的每次授权动作都会生成一个新的auth_code。
        /// </summary>
        [XmlElement("code")]
        public string Code { get; set; }

        /// <summary>
        /// 扩展参数，目前保留未用，开发者请忽略此参数
        /// </summary>
        [XmlElement("ext_info")]
        public string ExtInfo { get; set; }
    }
}

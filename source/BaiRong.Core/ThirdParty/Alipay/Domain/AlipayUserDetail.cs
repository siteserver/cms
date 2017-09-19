using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayUserDetail Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayUserDetail : AopObject
    {
        /// <summary>
        /// 支付宝用户userId
        /// </summary>
        [XmlElement("alipay_user_id")]
        public string AlipayUserId { get; set; }

        /// <summary>
        /// 是否通过实名认证
        /// </summary>
        [XmlElement("certified")]
        public bool Certified { get; set; }

        /// <summary>
        /// 支付宝登录号
        /// </summary>
        [XmlElement("logon_id")]
        public string LogonId { get; set; }

        /// <summary>
        /// 真实姓名
        /// </summary>
        [XmlElement("real_name")]
        public string RealName { get; set; }

        /// <summary>
        /// 性别。可选值:m(男),f(女)
        /// </summary>
        [XmlElement("sex")]
        public string Sex { get; set; }

        /// <summary>
        /// 用户状态。可选:normal(正常), supervise (监管),delete(注销)
        /// </summary>
        [XmlElement("user_status")]
        public string UserStatus { get; set; }

        /// <summary>
        /// 用户类型。可选：personal（个人），company（公司）
        /// </summary>
        [XmlElement("user_type")]
        public string UserType { get; set; }
    }
}

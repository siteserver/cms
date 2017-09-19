using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// NewsfeedWithMeInfo Data Structure.
    /// </summary>
    [Serializable]
    public class NewsfeedWithMeInfo : AopObject
    {
        /// <summary>
        /// 登录ID
        /// </summary>
        [XmlElement("login_id")]
        public string LoginId { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}

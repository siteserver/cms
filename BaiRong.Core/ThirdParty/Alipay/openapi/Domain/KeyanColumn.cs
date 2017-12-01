using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KeyanColumn Data Structure.
    /// </summary>
    [Serializable]
    public class KeyanColumn : AopObject
    {
        /// <summary>
        /// 密码
        /// </summary>
        [XmlElement("password")]
        public string Password { get; set; }
    }
}

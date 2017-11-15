using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AccountFreeze Data Structure.
    /// </summary>
    [Serializable]
    public class AccountFreeze : AopObject
    {
        /// <summary>
        /// 冻结金额
        /// </summary>
        [XmlElement("freeze_amount")]
        public string FreezeAmount { get; set; }

        /// <summary>
        /// 冻结类型名称
        /// </summary>
        [XmlElement("freeze_name")]
        public string FreezeName { get; set; }

        /// <summary>
        /// 冻结类型值
        /// </summary>
        [XmlElement("freeze_type")]
        public string FreezeType { get; set; }
    }
}

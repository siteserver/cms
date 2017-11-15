using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// SignResultValue Data Structure.
    /// </summary>
    [Serializable]
    public class SignResultValue : AopObject
    {
        /// <summary>
        /// 已生效的销账/出账机构
        /// </summary>
        [XmlElement("effect_biz_value")]
        public string EffectBizValue { get; set; }
    }
}

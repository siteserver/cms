using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ItemDiagnoseType Data Structure.
    /// </summary>
    [Serializable]
    public class ItemDiagnoseType : AopObject
    {
        /// <summary>
        /// 类型
        /// </summary>
        [XmlElement("item_diagnose")]
        public string ItemDiagnose { get; set; }

        /// <summary>
        /// 对类型的描述
        /// </summary>
        [XmlElement("item_diagnose_desc")]
        public string ItemDiagnoseDesc { get; set; }
    }
}

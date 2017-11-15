using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// MergeVerifyConfig Data Structure.
    /// </summary>
    [Serializable]
    public class MergeVerifyConfig : AopObject
    {
        /// <summary>
        /// 可以与任意其他券叠加
        /// </summary>
        [XmlElement("general_merge_others")]
        public bool GeneralMergeOthers { get; set; }

        /// <summary>
        /// 全场和单品叠加标识
        /// </summary>
        [XmlElement("general_merge_single")]
        public bool GeneralMergeSingle { get; set; }
    }
}

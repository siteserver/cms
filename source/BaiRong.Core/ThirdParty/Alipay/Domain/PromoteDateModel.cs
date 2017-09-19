using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// PromoteDateModel Data Structure.
    /// </summary>
    [Serializable]
    public class PromoteDateModel : AopObject
    {
        /// <summary>
        /// 日期
        /// </summary>
        [XmlElement("date")]
        public string Date { get; set; }

        /// <summary>
        /// 推广数据
        /// </summary>
        [XmlElement("promote_data")]
        public PromoteDataModel PromoteData { get; set; }
    }
}

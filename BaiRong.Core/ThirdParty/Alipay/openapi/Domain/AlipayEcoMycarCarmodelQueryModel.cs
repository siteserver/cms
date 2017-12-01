using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEcoMycarCarmodelQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEcoMycarCarmodelQueryModel : AopObject
    {
        /// <summary>
        /// 支付宝车型库车型编号,系统唯一。
        /// </summary>
        [XmlElement("model_id")]
        public string ModelId { get; set; }
    }
}

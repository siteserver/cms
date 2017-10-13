using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayInsCooperationProductQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayInsCooperationProductQueryModel : AopObject
    {
        /// <summary>
        /// 产品编码;由蚂蚁保险平台分配,商户通过该产品编码投保特定的保险产品
        /// </summary>
        [XmlElement("prod_code")]
        public string ProdCode { get; set; }
    }
}

using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayPcreditHuabeiPromoQueryResponse.
    /// </summary>
    public class AlipayPcreditHuabeiPromoQueryResponse : AopResponse
    {
        /// <summary>
        /// 花呗颜值分
        /// </summary>
        [XmlElement("facescore")]
        public string Facescore { get; set; }
    }
}

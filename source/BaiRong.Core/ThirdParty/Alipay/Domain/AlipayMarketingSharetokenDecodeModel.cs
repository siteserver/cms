using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayMarketingSharetokenDecodeModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayMarketingSharetokenDecodeModel : AopObject
    {
        /// <summary>
        /// 码类型，可空，默认为吱口令类型『share_code』
        /// </summary>
        [XmlElement("code_type")]
        public string CodeType { get; set; }

        /// <summary>
        /// 扩展属性，key-value json串
        /// </summary>
        [XmlElement("ext_data")]
        public string ExtData { get; set; }

        /// <summary>
        /// 8位吱口令token
        /// </summary>
        [XmlElement("token")]
        public string Token { get; set; }
    }
}

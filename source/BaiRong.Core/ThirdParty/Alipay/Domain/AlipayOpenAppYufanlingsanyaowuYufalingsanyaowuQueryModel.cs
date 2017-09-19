using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayOpenAppYufanlingsanyaowuYufalingsanyaowuQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayOpenAppYufanlingsanyaowuYufalingsanyaowuQueryModel : AopObject
    {
        /// <summary>
        /// 省份编码，国标码
        /// </summary>
        [XmlArray("province_code")]
        [XmlArrayItem("string")]
        public List<string> ProvinceCode { get; set; }
    }
}

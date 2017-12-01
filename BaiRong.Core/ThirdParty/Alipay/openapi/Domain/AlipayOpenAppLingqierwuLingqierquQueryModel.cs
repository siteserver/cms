using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayOpenAppLingqierwuLingqierquQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayOpenAppLingqierwuLingqierquQueryModel : AopObject
    {
        /// <summary>
        /// 12
        /// </summary>
        [XmlArray("test")]
        [XmlArrayItem("string")]
        public List<string> Test { get; set; }
    }
}

using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayZdataassetsMetadataResponse.
    /// </summary>
    public class AlipayZdataassetsMetadataResponse : AopResponse
    {
        /// <summary>
        /// 用户标签集合
        /// </summary>
        [XmlArray("result")]
        [XmlArrayItem("customer_entity")]
        public List<CustomerEntity> Result { get; set; }
    }
}

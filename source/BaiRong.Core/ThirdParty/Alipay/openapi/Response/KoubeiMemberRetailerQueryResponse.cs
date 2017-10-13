using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// KoubeiMemberRetailerQueryResponse.
    /// </summary>
    public class KoubeiMemberRetailerQueryResponse : AopResponse
    {
        /// <summary>
        /// 零售商信息列表
        /// </summary>
        [XmlArray("retailer_list")]
        [XmlArrayItem("retailer")]
        public List<Retailer> RetailerList { get; set; }
    }
}

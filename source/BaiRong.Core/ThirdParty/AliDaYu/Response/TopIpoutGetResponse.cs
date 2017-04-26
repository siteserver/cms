using System.Xml.Serialization;
using System.Collections.Generic;

namespace Top.Api.Response
{
    /// <summary>
    /// TopIpoutGetResponse.
    /// </summary>
    public class TopIpoutGetResponse : TopResponse
    {
        /// <summary>
        /// TOP网关出口IP列表
        /// </summary>
        [XmlArray("ip_list")]
        [XmlArrayItem("string")]
        public List<string> IpList { get; set; }

    }
}

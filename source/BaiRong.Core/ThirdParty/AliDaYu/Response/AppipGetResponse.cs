using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// AppipGetResponse.
    /// </summary>
    public class AppipGetResponse : TopResponse
    {
        /// <summary>
        /// ISV发起请求服务器IP
        /// </summary>
        [XmlElement("ip")]
        public string Ip { get; set; }

    }
}

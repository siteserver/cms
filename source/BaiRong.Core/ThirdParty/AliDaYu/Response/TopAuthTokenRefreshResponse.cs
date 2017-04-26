using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// TopAuthTokenRefreshResponse.
    /// </summary>
    public class TopAuthTokenRefreshResponse : TopResponse
    {
        /// <summary>
        /// 返回的是json信息
        /// </summary>
        [XmlElement("token_result")]
        public string TokenResult { get; set; }

    }
}

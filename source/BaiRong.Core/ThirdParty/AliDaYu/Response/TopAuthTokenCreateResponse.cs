using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// TopAuthTokenCreateResponse.
    /// </summary>
    public class TopAuthTokenCreateResponse : TopResponse
    {
        /// <summary>
        /// 返回的是json信息，和之前调用https://oauth.taobao.com/tac/token https://oauth.alibaba.com/token 换token返回的字段信息一致
        /// </summary>
        [XmlElement("token_result")]
        public string TokenResult { get; set; }

    }
}

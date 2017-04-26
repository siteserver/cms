using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// TopatsTaskDeleteResponse.
    /// </summary>
    public class TopatsTaskDeleteResponse : TopResponse
    {
        /// <summary>
        /// 表示操作是否成功，是为true，否为false。
        /// </summary>
        [XmlElement("is_success")]
        public bool IsSuccess { get; set; }

    }
}

using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// KfcKeywordSearchResponse.
    /// </summary>
    public class KfcKeywordSearchResponse : TopResponse
    {
        /// <summary>
        /// KFC 关键词过滤匹配结果
        /// </summary>
        [XmlElement("kfc_search_result")]
        public Top.Api.Domain.KfcSearchResult KfcSearchResult { get; set; }

    }
}

using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// AlibabaAliqinFcTtsNumSinglecallResponse.
    /// </summary>
    public class AlibabaAliqinFcTtsNumSinglecallResponse : TopResponse
    {
        /// <summary>
        /// 接口返回参数
        /// </summary>
        [XmlElement("result")]
        public Top.Api.Domain.BizResult Result { get; set; }

    }
}

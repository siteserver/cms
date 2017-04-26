using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// TopSdkFeedbackUploadResponse.
    /// </summary>
    public class TopSdkFeedbackUploadResponse : TopResponse
    {
        /// <summary>
        /// 控制回传间隔（单位：秒）
        /// </summary>
        [XmlElement("upload_interval")]
        public long UploadInterval { get; set; }

    }
}

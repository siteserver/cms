using System.Xml.Serialization;

namespace Top.Api.Report
{
    public class TopSdkFeedbackUploadResponse : TopResponse
    {

        /// <summary>
        ///控制回传间隔，单位（毫秒）
        /// </summary>
        [XmlElement("upload_interval")]
        public int UploadInterval { get; set; }

    }
}

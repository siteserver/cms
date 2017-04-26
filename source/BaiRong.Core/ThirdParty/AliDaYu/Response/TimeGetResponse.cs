using System.Xml.Serialization;

namespace Top.Api.Response
{
    /// <summary>
    /// TimeGetResponse.
    /// </summary>
    public class TimeGetResponse : TopResponse
    {
        /// <summary>
        /// 淘宝系统当前时间。格式:yyyy-MM-dd HH:mm:ss
        /// </summary>
        [XmlElement("time")]
        public string Time { get; set; }

    }
}

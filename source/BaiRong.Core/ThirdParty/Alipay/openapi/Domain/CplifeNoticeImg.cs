using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// CplifeNoticeImg Data Structure.
    /// </summary>
    [Serializable]
    public class CplifeNoticeImg : AopObject
    {
        /// <summary>
        /// 在通知中需要展示的图片链接，API调用之后，该图片将会被自动下载到物业社区平台服务器，用于系统显示。API调用成功之后，手工更改URL对应的图片资源时，用户在支付宝APP端看到的图片将保持不变。
        /// </summary>
        [XmlElement("image_url")]
        public string ImageUrl { get; set; }

        /// <summary>
        /// 在通知中需要展示的图片的缩略图链接，API调用之后，该图片将会被自动下载到物业社区平台服务器，用于系统显示。API调用成功之后，手工更改URL对应的图片资源时，用户在支付宝APP端看到的图片将保持不变。
        /// </summary>
        [XmlElement("thumbnail_url")]
        public string ThumbnailUrl { get; set; }
    }
}

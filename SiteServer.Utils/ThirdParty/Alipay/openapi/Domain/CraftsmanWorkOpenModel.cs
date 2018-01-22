using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// CraftsmanWorkOpenModel Data Structure.
    /// </summary>
    [Serializable]
    public class CraftsmanWorkOpenModel : AopObject
    {
        /// <summary>
        /// 口碑手艺人id。是创建手艺人接口koubei.craftsman.data.provider.create返回的craftsman_id，或通过查询手艺人信息接口koubei.craftsman.data.provider查询craftsman_id
        /// </summary>
        [XmlElement("craftsman_id")]
        public string CraftsmanId { get; set; }

        /// <summary>
        /// 视频资源必传 视频时长 单位（秒）
        /// </summary>
        [XmlElement("duration")]
        public long Duration { get; set; }

        /// <summary>
        /// 媒体资源id（通过 alipay.offline.material.image.upload 接口上传图片获取的资源id）。图片上限最大5M,支持bmp,png,jpeg,jpg,gif格式的图片。视频上限最大50M,支持MP4格式。
        /// </summary>
        [XmlElement("media_id")]
        public string MediaId { get; set; }

        /// <summary>
        /// 媒体类型。仅支持图片/视频格式，图片类型时为PICTURE；视频类型时为VIDEO
        /// </summary>
        [XmlElement("media_type")]
        public string MediaType { get; set; }

        /// <summary>
        /// 状态： EFFECTIVE 生效，INVALID 失效。失效状态主要用于平台对作品的处罚，例如作品涉黄赌毒、被用户投诉欺诈等。
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }

        /// <summary>
        /// 作品标题，上限10个字。
        /// </summary>
        [XmlElement("title")]
        public string Title { get; set; }

        /// <summary>
        /// 口碑手艺人作品id，是创建手艺人作品接口koubei.craftsman.data.work.create返回的work_id
        /// </summary>
        [XmlElement("work_id")]
        public string WorkId { get; set; }
    }
}

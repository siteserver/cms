using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayMarketingCdpAdvertiseQueryResponse.
    /// </summary>
    public class AlipayMarketingCdpAdvertiseQueryResponse : AopResponse
    {
        /// <summary>
        /// 用户点击广告后，跳转URL地址, 协议必须为HTTPS。广告类型为PIC时，需要设置该值。对于类型为URL不生效
        /// </summary>
        [XmlElement("action_url")]
        public string ActionUrl { get; set; }

        /// <summary>
        /// 广告位标识码，目前广告位只支持在支付宝钱包中显示，口碑app暂不支持。传值：CDP_OPEN_MERCHANT
        /// </summary>
        [XmlElement("ad_code")]
        public string AdCode { get; set; }

        /// <summary>
        /// 该规则用于商家设置对用户是否展示广告的校验条件，目前支持设置商家店铺规则。按业务要求应用对应规则即可
        /// </summary>
        [XmlElement("ad_rules")]
        public string AdRules { get; set; }

        /// <summary>
        /// 广告消失机制，CLOSE_AFTER_CLICK：总共点击一定次数消失
        /// </summary>
        [XmlElement("behavior")]
        public string Behavior { get; set; }

        /// <summary>
        /// 广告扩展字段
        /// </summary>
        [XmlElement("biz_ext_info")]
        public string BizExtInfo { get; set; }

        /// <summary>
        /// 广告内容。如果广告类型是HTML5，则传入H5链接地址，建议为https协议。最大尺寸不得超过1242px＊242px，小屏幕将按分辨率宽度同比例放大缩小；如果类型是图片，则传入图片ID标识，如何获取图片ID参考图片上传接口：alipay.offline.material.image.upload。图片尺寸为1242px＊290px。图片大小不能超过50kb。
        /// </summary>
        [XmlElement("content")]
        public string Content { get; set; }

        /// <summary>
        /// 广告内容类型，目前包括HTML5和图片，分别传入：URL和PIC
        /// </summary>
        [XmlElement("content_type")]
        public string ContentType { get; set; }

        /// <summary>
        /// 投放广告结束时间，使用标准时间格式：yyyy-MM-dd HH:mm:ss，如果不设置，默认投放时间一个月
        /// </summary>
        [XmlElement("end_time")]
        public string EndTime { get; set; }

        /// <summary>
        /// 当广告类型是H5时，必须传入内容高度。对于广告位CDP_OPEN_MERCHANT的内容高度不能高于钱包要求的展位高度242px。当广告类型是图片时，不需要传入内容高度(height)，系统会检查用户上传的图片尺寸是否符合要求，对于广告位CDP_OPEN_MERCHANT的图片尺寸要求：宽1242px, 高290px,大小50kb，实际上传图片与图片标准宽高必须一致，图片大小不能超过50kb。
        /// </summary>
        [XmlElement("height")]
        public string Height { get; set; }

        /// <summary>
        /// 投放广告开始时间，使用标准时间格式：yyyy-MM-dd HH:mm:ss，如果不设置，默认投放时间一个月
        /// </summary>
        [XmlElement("start_time")]
        public string StartTime { get; set; }

        /// <summary>
        /// 在线：ONLINE , 下线：OFFLINE
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }
    }
}

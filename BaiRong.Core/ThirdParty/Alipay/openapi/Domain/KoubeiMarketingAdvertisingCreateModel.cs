using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiMarketingAdvertisingCreateModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiMarketingAdvertisingCreateModel : AopObject
    {
        /// <summary>
        /// 用户点击广告后，跳转URL地址，必须为https协议。广告类型为PIC时，需要设置该值。对于类型为URL不生效。
        /// </summary>
        [XmlElement("action_url")]
        public string ActionUrl { get; set; }

        /// <summary>
        /// 广告位标识码，目前开放的广告位是钱包APP/口碑TAB/商家详情页中，传值：SHOPPING_OPEN_BANNER
        /// </summary>
        [XmlElement("ad_code")]
        public string AdCode { get; set; }

        /// <summary>
        /// 广告展示规则。该规则用于商家设置对用户是否展示广告的校验条件，目前支持商家店铺规则。按业务要求应用对应规则即可。
        /// </summary>
        [XmlElement("ad_rules")]
        public string AdRules { get; set; }

        /// <summary>
        /// 广告内容，目前支持传入图片ID标识。如何获取图片ID参考图片上传接口：alipay.offline.material.image.upload。图片尺寸为1242px＊290px。
        /// </summary>
        [XmlElement("content")]
        public string Content { get; set; }

        /// <summary>
        /// 广告类型，目前只支持PIC.
        /// </summary>
        [XmlElement("content_type")]
        public string ContentType { get; set; }

        /// <summary>
        /// 投放广告结束时间，使用标准时间格式：yyyy-MM-dd HH:mm:ss，如果不设置，默认投放时间一个月
        /// </summary>
        [XmlElement("end_time")]
        public string EndTime { get; set; }

        /// <summary>
        /// 直接传入的是图片，目前该字段可以先忽略
        /// </summary>
        [XmlElement("height")]
        public string Height { get; set; }

        /// <summary>
        /// 投放广告开始时间，使用标准时间格式：yyyy-MM-dd HH:mm:ss，如果不设置，默认投放时间一个月
        /// </summary>
        [XmlElement("start_time")]
        public string StartTime { get; set; }
    }
}

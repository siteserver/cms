using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayOfflineMarketReporterrorCreateModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayOfflineMarketReporterrorCreateModel : AopObject
    {
        /// <summary>
        /// 发生错误的时候，当前系统的毫秒数，系统会把当前时间构建成Date对象保存为错误发生时间
        /// </summary>
        [XmlElement("err_time")]
        public long ErrTime { get; set; }

        /// <summary>
        /// 如果：type是tableNum  请设置table_num字段作为桌码
        /// </summary>
        [XmlElement("feature")]
        public ReportErrorFeature Feature { get; set; }

        /// <summary>
        /// 商户ID
        /// </summary>
        [XmlElement("merchant_id")]
        public string MerchantId { get; set; }

        /// <summary>
        /// 口碑门店ID
        /// </summary>
        [XmlElement("shop_id")]
        public string ShopId { get; set; }

        /// <summary>
        /// 上传类型，通过类型来区分不同错误： value=tableNum 代表扫码点菜
        /// </summary>
        [XmlElement("type")]
        public string Type { get; set; }

        /// <summary>
        /// 用户的ID
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}

using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KbAdvertContentPasswordModify Data Structure.
    /// </summary>
    [Serializable]
    public class KbAdvertContentPasswordModify : AopObject
    {
        /// <summary>
        /// 口令红包背景图的django ID
        /// </summary>
        [XmlElement("background_img_id")]
        public string BackgroundImgId { get; set; }

        /// <summary>
        /// 口令红包品牌名称（品牌名称不能超过20位）
        /// </summary>
        [XmlElement("brand_name")]
        public string BrandName { get; set; }

        /// <summary>
        /// 红包口令（口令不能超过20位，口令只能是中文、英文、数字组合,不能纯数字）
        /// </summary>
        [XmlElement("password")]
        public string Password { get; set; }

        /// <summary>
        /// 口令红包券LOGO的django ID
        /// </summary>
        [XmlElement("voucher_logo_id")]
        public string VoucherLogoId { get; set; }
    }
}

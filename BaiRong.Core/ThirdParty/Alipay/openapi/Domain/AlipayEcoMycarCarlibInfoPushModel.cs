using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEcoMycarCarlibInfoPushModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEcoMycarCarlibInfoPushModel : AopObject
    {
        /// <summary>
        /// 品牌
        /// </summary>
        [XmlElement("brand")]
        public string Brand { get; set; }

        /// <summary>
        /// 排量
        /// </summary>
        [XmlElement("cc")]
        public string Cc { get; set; }

        /// <summary>
        /// 厂商
        /// </summary>
        [XmlElement("company")]
        public string Company { get; set; }

        /// <summary>
        /// 发动机型号
        /// </summary>
        [XmlElement("engine")]
        public string Engine { get; set; }

        /// <summary>
        /// 销售名字
        /// </summary>
        [XmlElement("marker")]
        public string Marker { get; set; }

        /// <summary>
        /// 生产年份
        /// </summary>
        [XmlElement("prod_year")]
        public string ProdYear { get; set; }

        /// <summary>
        /// 车系
        /// </summary>
        [XmlElement("serie")]
        public string Serie { get; set; }

        /// <summary>
        /// 车款
        /// </summary>
        [XmlElement("style")]
        public string Style { get; set; }
    }
}

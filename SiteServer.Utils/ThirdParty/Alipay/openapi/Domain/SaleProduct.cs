using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// SaleProduct Data Structure.
    /// </summary>
    [Serializable]
    public class SaleProduct : AopObject
    {
        /// <summary>
        /// 宝贝来源 例如：TAOBAO ALIPAY
        /// </summary>
        [XmlElement("channel_type")]
        public string ChannelType { get; set; }

        /// <summary>
        /// 销售产品DB ID
        /// </summary>
        [XmlElement("id")]
        public string Id { get; set; }

        /// <summary>
        /// 面额
        /// </summary>
        [XmlElement("market_price")]
        public string MarketPrice { get; set; }

        /// <summary>
        /// 充值产品提供商
        /// </summary>
        [XmlElement("product_provider")]
        public ProductProvider ProductProvider { get; set; }

        /// <summary>
        /// 销售价格
        /// </summary>
        [XmlElement("sale_price")]
        public string SalePrice { get; set; }

        /// <summary>
        /// 产品状态, 0为不可用，1为可用
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }
    }
}

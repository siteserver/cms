using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// InvoiceTradeGoodsItem Data Structure.
    /// </summary>
    [Serializable]
    public class InvoiceTradeGoodsItem : AopObject
    {
        /// <summary>
        /// 商品类目
        /// </summary>
        [XmlElement("category")]
        public string Category { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [XmlElement("goods_name")]
        public string GoodsName { get; set; }

        /// <summary>
        /// 商户设置的商品编号
        /// </summary>
        [XmlElement("goods_no")]
        public string GoodsNo { get; set; }

        /// <summary>
        /// 商品项支付金额
        /// </summary>
        [XmlElement("goods_sum_amount")]
        public string GoodsSumAmount { get; set; }

        /// <summary>
        /// 商品单价，单位元，精确到小数点后两位
        /// </summary>
        [XmlElement("price")]
        public string Price { get; set; }

        /// <summary>
        /// 购买数量
        /// </summary>
        [XmlElement("quantity")]
        public string Quantity { get; set; }

        /// <summary>
        /// 购买商品规格型号描述
        /// </summary>
        [XmlElement("specification")]
        public string Specification { get; set; }

        /// <summary>
        /// 购买商品单位描述
        /// </summary>
        [XmlElement("unit")]
        public string Unit { get; set; }
    }
}

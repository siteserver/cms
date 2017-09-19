using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// InvoiceItemQueryOpenModel Data Structure.
    /// </summary>
    [Serializable]
    public class InvoiceItemQueryOpenModel : AopObject
    {
        /// <summary>
        /// 不含税金额
        /// </summary>
        [XmlElement("item_ex_tax_amount")]
        public string ItemExTaxAmount { get; set; }

        /// <summary>
        /// 发票项目名称（或商品名称）
        /// </summary>
        [XmlElement("item_name")]
        public string ItemName { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        [XmlElement("item_no")]
        public string ItemNo { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [XmlElement("item_quantity")]
        public string ItemQuantity { get; set; }

        /// <summary>
        /// 商品型号
        /// </summary>
        [XmlElement("item_spec")]
        public string ItemSpec { get; set; }

        /// <summary>
        /// 价税合计。(等于item_tax_amount和item_ex_tax_amount之和）
        /// </summary>
        [XmlElement("item_sum_amount")]
        public string ItemSumAmount { get; set; }

        /// <summary>
        /// 税额
        /// </summary>
        [XmlElement("item_tax_amount")]
        public string ItemTaxAmount { get; set; }

        /// <summary>
        /// 税率
        /// </summary>
        [XmlElement("item_tax_rate")]
        public string ItemTaxRate { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        [XmlElement("item_unit")]
        public string ItemUnit { get; set; }

        /// <summary>
        /// 单价，格式：100.00。新版电子发票，折扣行此参数不能传，非折扣行必传
        /// </summary>
        [XmlElement("item_unit_price")]
        public string ItemUnitPrice { get; set; }

        /// <summary>
        /// 发票行性质。0表示正常行，1表示折扣行，2表示被折扣行。比如充电器单价100元，折扣10元，则明细为2行，充电器行性质为2，折扣行性质为1。如果充电器没有折扣，则值应为0。
        /// </summary>
        [XmlElement("row_type")]
        public string RowType { get; set; }
    }
}

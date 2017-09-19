using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AccessPurchaseOrderSend Data Structure.
    /// </summary>
    [Serializable]
    public class AccessPurchaseOrderSend : AopObject
    {
        /// <summary>
        /// 申请单明细号
        /// </summary>
        [XmlElement("asset_item_id")]
        public string AssetItemId { get; set; }

        /// <summary>
        /// 申请单号
        /// </summary>
        [XmlElement("asset_order_id")]
        public string AssetOrderId { get; set; }

        /// <summary>
        /// 采购单ID
        /// </summary>
        [XmlElement("asset_purchase_id")]
        public string AssetPurchaseId { get; set; }

        /// <summary>
        /// 物流公司code
        /// </summary>
        [XmlElement("express_code")]
        public string ExpressCode { get; set; }

        /// <summary>
        /// 物流公司名称
        /// </summary>
        [XmlElement("express_name")]
        public string ExpressName { get; set; }

        /// <summary>
        /// 物流单号
        /// </summary>
        [XmlElement("express_no")]
        public string ExpressNo { get; set; }

        /// <summary>
        /// 外部单号（供应商主键标识）
        /// </summary>
        [XmlElement("out_biz_no")]
        public string OutBizNo { get; set; }

        /// <summary>
        /// PO单号
        /// </summary>
        [XmlElement("po_no")]
        public string PoNo { get; set; }

        /// <summary>
        /// 回传码值数量1000(不是码物料时为0)
        /// </summary>
        [XmlElement("return_qrcode_count")]
        public string ReturnQrcodeCount { get; set; }

        /// <summary>
        /// 本次发货数量
        /// </summary>
        [XmlElement("ship_count")]
        public string ShipCount { get; set; }
    }
}

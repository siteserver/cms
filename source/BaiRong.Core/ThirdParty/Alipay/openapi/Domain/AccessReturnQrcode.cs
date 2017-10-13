using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AccessReturnQrcode Data Structure.
    /// </summary>
    [Serializable]
    public class AccessReturnQrcode : AopObject
    {
        /// <summary>
        /// 采购单ID
        /// </summary>
        [XmlElement("asset_purchase_id")]
        public string AssetPurchaseId { get; set; }

        /// <summary>
        /// 物流单号
        /// </summary>
        [XmlElement("express_no")]
        public string ExpressNo { get; set; }

        /// <summary>
        /// 外部单号（调用方业务主键）
        /// </summary>
        [XmlElement("out_biz_no")]
        public string OutBizNo { get; set; }

        /// <summary>
        /// 二维码token值
        /// </summary>
        [XmlElement("qrcode")]
        public string Qrcode { get; set; }
    }
}

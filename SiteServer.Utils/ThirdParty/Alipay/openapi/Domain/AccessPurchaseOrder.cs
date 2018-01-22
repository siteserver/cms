using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AccessPurchaseOrder Data Structure.
    /// </summary>
    [Serializable]
    public class AccessPurchaseOrder : AopObject
    {
        /// <summary>
        /// 申请日期, 格式:  yyyy-MM-dd HH:mm:ss
        /// </summary>
        [XmlElement("apply_date")]
        public string ApplyDate { get; set; }

        /// <summary>
        /// 申请订单明细号
        /// </summary>
        [XmlElement("asset_item_id")]
        public string AssetItemId { get; set; }

        /// <summary>
        /// 申请订单号
        /// </summary>
        [XmlElement("asset_order_id")]
        public string AssetOrderId { get; set; }

        /// <summary>
        /// 采购单号（订单汇总表ID）
        /// </summary>
        [XmlElement("asset_purchase_id")]
        public string AssetPurchaseId { get; set; }

        /// <summary>
        /// 市
        /// </summary>
        [XmlElement("city")]
        public string City { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [XmlElement("count")]
        public string Count { get; set; }

        /// <summary>
        /// 订单创建日期, 格式: yyyy-MM-dd HH:mm:ss
        /// </summary>
        [XmlElement("create_date")]
        public string CreateDate { get; set; }

        /// <summary>
        /// 区
        /// </summary>
        [XmlElement("district")]
        public string District { get; set; }

        /// <summary>
        /// 是否需要生产
        /// </summary>
        [XmlElement("is_produce")]
        public string IsProduce { get; set; }

        /// <summary>
        /// 省
        /// </summary>
        [XmlElement("province")]
        public string Province { get; set; }

        /// <summary>
        /// 收货人地址
        /// </summary>
        [XmlElement("receiver_address")]
        public string ReceiverAddress { get; set; }

        /// <summary>
        /// 联系人电话
        /// </summary>
        [XmlElement("receiver_mobile")]
        public string ReceiverMobile { get; set; }

        /// <summary>
        /// 收货人姓名
        /// </summary>
        [XmlElement("receiver_name")]
        public string ReceiverName { get; set; }

        /// <summary>
        /// 物料类型
        /// </summary>
        [XmlElement("stuff_attr_name")]
        public string StuffAttrName { get; set; }

        /// <summary>
        /// 物料材质
        /// </summary>
        [XmlElement("stuff_material")]
        public string StuffMaterial { get; set; }

        /// <summary>
        /// 物料尺寸
        /// </summary>
        [XmlElement("stuff_size")]
        public string StuffSize { get; set; }

        /// <summary>
        /// 物料属性
        /// </summary>
        [XmlElement("stuff_type")]
        public string StuffType { get; set; }

        /// <summary>
        /// 模板ID
        /// </summary>
        [XmlElement("template_id")]
        public string TemplateId { get; set; }

        /// <summary>
        /// 模板名称，线下约定的物料名
        /// </summary>
        [XmlElement("template_name")]
        public string TemplateName { get; set; }
    }
}

using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayOfflineMarketItemCreateModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayOfflineMarketItemCreateModel : AopObject
    {
        /// <summary>
        /// 商品审核上下文。支付宝内部使用，外部商户不需填写此字段
        /// </summary>
        [XmlElement("audit_rule")]
        public AlipayItemAuditRule AuditRule { get; set; }

        /// <summary>
        /// 商品首图，尺寸比例在65:53范围内且图片大小不超过10k皆可，图片推荐尺寸540*420
        /// </summary>
        [XmlElement("cover")]
        public string Cover { get; set; }

        /// <summary>
        /// 商品描述（代金券时，此字段必填）
        /// </summary>
        [XmlArray("descriptions")]
        [XmlArrayItem("alipay_item_description")]
        public List<AlipayItemDescription> Descriptions { get; set; }

        /// <summary>
        /// 商品下架时间，不得早于商品生效时间，商品下架
        /// </summary>
        [XmlElement("gmt_end")]
        public string GmtEnd { get; set; }

        /// <summary>
        /// 商品生效时间，到达生效时间后才可在客户端展示出来。  说明： 商品的生效时间不能早于创建当天的0点
        /// </summary>
        [XmlElement("gmt_start")]
        public string GmtStart { get; set; }

        /// <summary>
        /// 商品库存数量
        /// </summary>
        [XmlElement("inventory")]
        public long Inventory { get; set; }

        /// <summary>
        /// 是否自动延期，默认false。  如果需要设置自动延期，则gmt_start和gmt_end之间要间隔2天以上
        /// </summary>
        [XmlElement("is_auto_expanded")]
        public bool IsAutoExpanded { get; set; }

        /// <summary>
        /// 商品类型，券类型填写固定值VOUCHER
        /// </summary>
        [XmlElement("item_type")]
        public string ItemType { get; set; }

        /// <summary>
        /// 商户通知地址，口碑发消息给商户通知其是否对商品创建、修改、变更状态成功
        /// </summary>
        [XmlElement("operate_notify_url")]
        public string OperateNotifyUrl { get; set; }

        /// <summary>
        /// 商品操作上下文。支付宝内部使用，外部商户不需填写此字段。
        /// </summary>
        [XmlElement("operation_context")]
        public AlipayItemOperationContext OperationContext { get; set; }

        /// <summary>
        /// 商品购买类型 OBTAIN为领取，AUTO_OBTAIN为自动领取
        /// </summary>
        [XmlElement("purchase_mode")]
        public string PurchaseMode { get; set; }

        /// <summary>
        /// 支持英文字母和数字，由开发者自行定义（不允许重复），在商品notify消息中也会带有该参数，以此标明本次notify消息是对哪个请求的回应
        /// </summary>
        [XmlElement("request_id")]
        public string RequestId { get; set; }

        /// <summary>
        /// 销售规则
        /// </summary>
        [XmlElement("sales_rule")]
        public AlipayItemSalesRule SalesRule { get; set; }

        /// <summary>
        /// 上架门店id列表，即传入一个或多个shop_id，必须是创建商品partnerId下的店铺，目前支持的店铺最大100个，如果超过100个店铺需要报备
        /// </summary>
        [XmlElement("shop_list")]
        public string ShopList { get; set; }

        /// <summary>
        /// 商品名称，请勿超过15个汉字，30个字符
        /// </summary>
        [XmlElement("subject")]
        public string Subject { get; set; }

        /// <summary>
        /// 券模板信息
        /// </summary>
        [XmlElement("voucher_templete")]
        public AlipayItemVoucherTemplete VoucherTemplete { get; set; }

        /// <summary>
        /// 商品顺序权重，必须是整数，不传默认为0，权重数值越大排序越靠前
        /// </summary>
        [XmlElement("weight")]
        public long Weight { get; set; }
    }
}

using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiAdvertDeliveryDiscountBatchqueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiAdvertDeliveryDiscountBatchqueryModel : AopObject
    {
        /// <summary>
        /// 媒体编号，使用前需要找业务申请 ，不申请直接调用会失败
        /// </summary>
        [XmlElement("channel")]
        public string Channel { get; set; }

        /// <summary>
        /// 城市国标码，比如310100是上海
        /// </summary>
        [XmlElement("city_code")]
        public string CityCode { get; set; }

        /// <summary>
        /// 纬度
        /// </summary>
        [XmlElement("latitude")]
        public string Latitude { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        [XmlElement("longitude")]
        public string Longitude { get; set; }

        /// <summary>
        /// 手机号码，不能和user_id同时存在
        /// </summary>
        [XmlElement("mobile")]
        public string Mobile { get; set; }

        /// <summary>
        /// 当strategy为QUERY_AND_PURCHASE时，循环发送券列表中的券，直到发券量达到purchase_num。
        /// </summary>
        [XmlElement("purchase_num")]
        public string PurchaseNum { get; set; }

        /// <summary>
        /// 门店ID  如果提供门店ID，则优先查询门店下发的券。
        /// </summary>
        [XmlElement("shop_id")]
        public string ShopId { get; set; }

        /// <summary>
        /// 输出的券列表大小
        /// </summary>
        [XmlElement("size")]
        public string Size { get; set; }

        /// <summary>
        /// 查询策略，查询并发送策略，只有白名单内ISV才有权限使用，如果不在白名单内，则忽略该字段并默认查询  QUERY：查券（默认值）  QUERY_AND_PURCHASE：查并领，为了优化体验，在查询时进行发券处理，顺序发送券列表的券，直达发券量达到purchase_num。
        /// </summary>
        [XmlElement("strategy")]
        public string Strategy { get; set; }

        /// <summary>
        /// 支付宝账户ID
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}

using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayUserUnicomOrderInfoSyncModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayUserUnicomOrderInfoSyncModel : AopObject
    {
        /// <summary>
        /// 订单变更时间，返回自January 1, 1970, 00:00:00 GMT至订单变更时刻的毫秒数, java代码获取示例：new Date().getTime()
        /// </summary>
        [XmlElement("gmt_order_change")]
        public string GmtOrderChange { get; set; }

        /// <summary>
        /// 用户在联通开卡创建的订单号
        /// </summary>
        [XmlElement("order_no")]
        public string OrderNo { get; set; }

        /// <summary>
        /// 订单变更类型，包括创建(ORDER_CREATE)、撤销(ORDER_CANCEL)、支付(ORDER_PAID)等
        /// </summary>
        [XmlElement("order_operate_type")]
        public string OrderOperateType { get; set; }

        /// <summary>
        /// 用户在创建开卡订单时选择的联通号码（11位，不带国家码）
        /// </summary>
        [XmlElement("phone_no")]
        public string PhoneNo { get; set; }

        /// <summary>
        /// 联通资费产品名称
        /// </summary>
        [XmlElement("product_name")]
        public string ProductName { get; set; }

        /// <summary>
        /// 参数校验值
        /// </summary>
        [XmlElement("sec_key")]
        public string SecKey { get; set; }

        /// <summary>
        /// 支付宝用户ID
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}

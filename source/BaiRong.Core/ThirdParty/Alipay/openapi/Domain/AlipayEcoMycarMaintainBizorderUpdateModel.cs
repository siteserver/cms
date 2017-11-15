using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEcoMycarMaintainBizorderUpdateModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEcoMycarMaintainBizorderUpdateModel : AopObject
    {
        /// <summary>
        /// 预约确认时间yyyy-MM-dd HH:mm:ss。门店确认预约时间。门店确认后，预约流程生效，用户可到店服务。
        /// </summary>
        [XmlElement("appoint_affirm_time")]
        public string AppointAffirmTime { get; set; }

        /// <summary>
        /// 预约结束时间yyyy-MM-dd HH:mm:ss。用户选择的预约结束时间，用于判断用户是否在预约时间到店服务。
        /// </summary>
        [XmlElement("appoint_end_time")]
        public string AppointEndTime { get; set; }

        /// <summary>
        /// 预约开始时间yyyy-MM-dd HH:mm:ss，用户选择的预约开始时间，用于判断用户是否在预约时间到店服务。
        /// </summary>
        [XmlElement("appoint_start_time")]
        public string AppointStartTime { get; set; }

        /// <summary>
        /// 预约状态(0-待确认预约 1-确认预约)。有预约流程的订单，门店确认前为待确认预约，门店确认后为 确认预约。
        /// </summary>
        [XmlElement("appoint_status")]
        public long AppointStatus { get; set; }

        /// <summary>
        /// 到店时间yyyy-MM-dd HH:mm:ss。 用户到店时间，用于判断用户是否在预约时间到店服务。
        /// </summary>
        [XmlElement("arrive_time")]
        public string ArriveTime { get; set; }

        /// <summary>
        /// 车主平台订单编号
        /// </summary>
        [XmlElement("order_no")]
        public string OrderNo { get; set; }

        /// <summary>
        /// 服务项列表
        /// </summary>
        [XmlArray("order_server_list")]
        [XmlArrayItem("maintain_biz_order")]
        public List<MaintainBizOrder> OrderServerList { get; set; }

        /// <summary>
        /// 原始金额。服务原价累计后金额。服务项变更时，重新计算交易金额。金额单位(元)，保留两位小数。原始金额 = 服务原始价格 * 数量 + 商品售卖价格 * 数量
        /// </summary>
        [XmlElement("original_cost")]
        public string OriginalCost { get; set; }

        /// <summary>
        /// 外部门店编号；更换门店下单是门店编号，适用于门店重新指派场景。
        /// </summary>
        [XmlElement("out_shop_id")]
        public string OutShopId { get; set; }

        /// <summary>
        /// 交易金额。服务项变更时，重新计算交易金额。金额单位(元)，保留两位小数。交易金额 = 服务售卖价格 * 数量 + 商品售卖价格 * 数量
        /// </summary>
        [XmlElement("real_cost")]
        public string RealCost { get; set; }

        /// <summary>
        /// 修改场景类型：  appoint_change: 变更预约时间，  appoint_affirm : 预约确认  shop_arrive :到店  service_change : 服务项修改  shop_change : 门店重新指派    变更预约时间（appoint_change）  涉及字段：appoint_start_time、 appoint_end_time  条件：订单未支付、已支付，未确定预约。    确认预约（appoint_affirm）  涉及字段：appoint_status、appoint_affirm_time  条件：订单已支付、未到店    确认到店（shop_arrive）  涉及字段：arrive_time  条件：订单已确定预约、服务未完成    服务项变更（service_change）  涉及字段：order_server_list、real_cost、original_cost  条件：开始保养，服务未完成    门店重新指派（shop_change）  涉及字段：out_shop_id  条件：服务未完成
        /// </summary>
        [XmlElement("scene_type")]
        public string SceneType { get; set; }
    }
}

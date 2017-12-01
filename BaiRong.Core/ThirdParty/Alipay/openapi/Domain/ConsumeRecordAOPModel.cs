using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ConsumeRecordAOPModel Data Structure.
    /// </summary>
    [Serializable]
    public class ConsumeRecordAOPModel : AopObject
    {
        /// <summary>
        /// 接入渠道
        /// </summary>
        [XmlElement("access_channel")]
        public string AccessChannel { get; set; }

        /// <summary>
        /// 扩展状态
        /// </summary>
        [XmlElement("additional_status")]
        public string AdditionalStatus { get; set; }

        /// <summary>
        /// 用户下一步动作
        /// </summary>
        [XmlArray("biz_actions")]
        [XmlArrayItem("string")]
        public List<string> BizActions { get; set; }

        /// <summary>
        /// 业务扩展流水号
        /// </summary>
        [XmlElement("biz_extra_no")]
        public string BizExtraNo { get; set; }

        /// <summary>
        /// 业务流水号
        /// </summary>
        [XmlElement("biz_in_no")]
        public string BizInNo { get; set; }

        /// <summary>
        /// 业务备注
        /// </summary>
        [XmlElement("biz_memo")]
        public string BizMemo { get; set; }

        /// <summary>
        /// 业务发起方
        /// </summary>
        [XmlElement("biz_orig")]
        public string BizOrig { get; set; }

        /// <summary>
        /// 业务外部流水号
        /// </summary>
        [XmlElement("biz_out_no")]
        public string BizOutNo { get; set; }

        /// <summary>
        /// 业务状态
        /// </summary>
        [XmlElement("biz_state")]
        public string BizState { get; set; }

        /// <summary>
        /// 业务子类型
        /// </summary>
        [XmlElement("biz_sub_type")]
        public string BizSubType { get; set; }

        /// <summary>
        /// 业务类型
        /// </summary>
        [XmlElement("biz_type")]
        public string BizType { get; set; }

        /// <summary>
        /// 自定义分类Id
        /// </summary>
        [XmlElement("category_id")]
        public string CategoryId { get; set; }

        /// <summary>
        /// 总费用
        /// </summary>
        [XmlElement("consume_fee")]
        public string ConsumeFee { get; set; }

        /// <summary>
        /// 消费记录退款状态
        /// </summary>
        [XmlElement("consume_refund_status")]
        public string ConsumeRefundStatus { get; set; }

        /// <summary>
        /// 来源
        /// </summary>
        [XmlElement("consume_site")]
        public string ConsumeSite { get; set; }

        /// <summary>
        /// 消费记录状态
        /// </summary>
        [XmlElement("consume_status")]
        public string ConsumeStatus { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        [XmlElement("consume_title")]
        public string ConsumeTitle { get; set; }

        /// <summary>
        /// 消费记录类型
        /// </summary>
        [XmlElement("consume_type")]
        public string ConsumeType { get; set; }

        /// <summary>
        /// 货币币种
        /// </summary>
        [XmlElement("currency")]
        public string Currency { get; set; }

        /// <summary>
        /// 消费记录永久删除时间
        /// </summary>
        [XmlElement("delete_over_time")]
        public string DeleteOverTime { get; set; }

        /// <summary>
        /// 消费记录删除时间
        /// </summary>
        [XmlElement("delete_time")]
        public string DeleteTime { get; set; }

        /// <summary>
        /// 消费记录删除标记
        /// </summary>
        [XmlElement("delete_type")]
        public string DeleteType { get; set; }

        /// <summary>
        /// 充退状态，可能为空，目前只针对交易业务有效
        /// </summary>
        [XmlElement("depositback_status")]
        public string DepositbackStatus { get; set; }

        /// <summary>
        /// 是否锁定标记
        /// </summary>
        [XmlElement("flag_locked")]
        public string FlagLocked { get; set; }

        /// <summary>
        /// 退款标记
        /// </summary>
        [XmlElement("flag_refund")]
        public string FlagRefund { get; set; }

        /// <summary>
        /// 业务数据创建时间
        /// </summary>
        [XmlElement("gmt_biz_create")]
        public string GmtBizCreate { get; set; }

        /// <summary>
        /// 业务数据最后更新时间
        /// </summary>
        [XmlElement("gmt_biz_modified")]
        public string GmtBizModified { get; set; }

        /// <summary>
        /// 消费记录创建时间
        /// </summary>
        [XmlElement("gmt_create")]
        public string GmtCreate { get; set; }

        /// <summary>
        /// 消费记录最后更新时间
        /// </summary>
        [XmlElement("gmt_modified")]
        public string GmtModified { get; set; }

        /// <summary>
        /// 收到付款时间（买家付款时间）
        /// </summary>
        [XmlElement("gmt_receive_pay")]
        public string GmtReceivePay { get; set; }

        /// <summary>
        /// 打款给卖家时间（卖家收款时间）
        /// </summary>
        [XmlElement("gmt_send_pay")]
        public string GmtSendPay { get; set; }

        /// <summary>
        /// 是否有资金明细
        /// </summary>
        [XmlElement("has_fund_item")]
        public bool HasFundItem { get; set; }

        /// <summary>
        /// 是否有新资金明细(落地新流程交易的消费记录)
        /// </summary>
        [XmlElement("has_new_fund_item")]
        public bool HasNewFundItem { get; set; }

        /// <summary>
        /// 收入/支出
        /// </summary>
        [XmlElement("in_out")]
        public string InOut { get; set; }

        /// <summary>
        /// 对方卡号
        /// </summary>
        [XmlElement("opposite_card_no")]
        public string OppositeCardNo { get; set; }

        /// <summary>
        /// 对方登录ID
        /// </summary>
        [XmlElement("opposite_login_id")]
        public string OppositeLoginId { get; set; }

        /// <summary>
        /// 对方名称
        /// </summary>
        [XmlElement("opposite_name")]
        public string OppositeName { get; set; }

        /// <summary>
        /// 对方昵称
        /// </summary>
        [XmlElement("opposite_nick_name")]
        public string OppositeNickName { get; set; }

        /// <summary>
        /// 消费记录原标题
        /// </summary>
        [XmlElement("orig_consume_title")]
        public string OrigConsumeTitle { get; set; }

        /// <summary>
        /// 本方卡号
        /// </summary>
        [XmlElement("owner_card_no")]
        public string OwnerCardNo { get; set; }

        /// <summary>
        /// 本方客户ID
        /// </summary>
        [XmlElement("owner_customer_id")]
        public string OwnerCustomerId { get; set; }

        /// <summary>
        /// 本方登录ID
        /// </summary>
        [XmlElement("owner_login_id")]
        public string OwnerLoginId { get; set; }

        /// <summary>
        /// 本方名称
        /// </summary>
        [XmlElement("owner_name")]
        public string OwnerName { get; set; }

        /// <summary>
        /// 本方昵称
        /// </summary>
        [XmlElement("owner_nick")]
        public string OwnerNick { get; set; }

        /// <summary>
        /// 合作伙伴ID
        /// </summary>
        [XmlElement("partner_id")]
        public string PartnerId { get; set; }

        /// <summary>
        /// 创建渠道
        /// </summary>
        [XmlElement("pay_channel")]
        public string PayChannel { get; set; }

        /// <summary>
        /// 代付人&亲密付人姓名
        /// </summary>
        [XmlElement("peerpayer_real_name")]
        public string PeerpayerRealName { get; set; }

        /// <summary>
        /// 产品码
        /// </summary>
        [XmlElement("product")]
        public string Product { get; set; }

        /// <summary>
        /// 最后一次退款金额
        /// </summary>
        [XmlElement("refund_fee")]
        public string RefundFee { get; set; }

        /// <summary>
        /// 服务费
        /// </summary>
        [XmlElement("service_fee")]
        public string ServiceFee { get; set; }

        /// <summary>
        /// 多次成功退款总金额
        /// </summary>
        [XmlElement("total_refund_fee")]
        public string TotalRefundFee { get; set; }

        /// <summary>
        /// 交易来源
        /// </summary>
        [XmlElement("trade_from")]
        public string TradeFrom { get; set; }
    }
}

using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// FundItemAOPModel Data Structure.
    /// </summary>
    [Serializable]
    public class FundItemAOPModel : AopObject
    {
        /// <summary>
        /// 财务外部单据号
        /// </summary>
        [XmlElement("acctrans_out_biz_no")]
        public string AcctransOutBizNo { get; set; }

        /// <summary>
        /// 资金变动金额
        /// </summary>
        [XmlElement("amount")]
        public string Amount { get; set; }

        /// <summary>
        /// 资产类型编码
        /// </summary>
        [XmlElement("assert_type_code")]
        public string AssertTypeCode { get; set; }

        /// <summary>
        /// 银行卡支付工具类型
        /// </summary>
        [XmlElement("bank_card_pay_type")]
        public string BankCardPayType { get; set; }

        /// <summary>
        /// 银行卡类型
        /// </summary>
        [XmlElement("bank_card_type")]
        public string BankCardType { get; set; }

        /// <summary>
        /// 业务ID
        /// </summary>
        [XmlElement("biz_id")]
        public string BizId { get; set; }

        /// <summary>
        /// 业务号
        /// </summary>
        [XmlElement("biz_in_no")]
        public string BizInNo { get; set; }

        /// <summary>
        /// 业务外部流水号
        /// </summary>
        [XmlElement("biz_out_no")]
        public string BizOutNo { get; set; }

        /// <summary>
        /// 业务类型（枚举值对应的code）
        /// </summary>
        [XmlElement("biz_type")]
        public string BizType { get; set; }

        /// <summary>
        /// 币种（数字形式）
        /// </summary>
        [XmlElement("currency")]
        public string Currency { get; set; }

        /// <summary>
        /// 充退金额
        /// </summary>
        [XmlElement("dback_amount")]
        public string DbackAmount { get; set; }

        /// <summary>
        /// 退款申请时间
        /// </summary>
        [XmlElement("dback_gmt_create")]
        public string DbackGmtCreate { get; set; }

        /// <summary>
        /// 实际/预估银行响应时间
        /// </summary>
        [XmlElement("dback_gmt_est_bk_ack")]
        public string DbackGmtEstBkAck { get; set; }

        /// <summary>
        /// 预估银行入账时间
        /// </summary>
        [XmlElement("dback_gmt_est_bk_into")]
        public string DbackGmtEstBkInto { get; set; }

        /// <summary>
        /// 充值账户名
        /// </summary>
        [XmlElement("dback_inst_account_name")]
        public string DbackInstAccountName { get; set; }

        /// <summary>
        /// 充值卡号（后四位）
        /// </summary>
        [XmlElement("dback_inst_account_no")]
        public string DbackInstAccountNo { get; set; }

        /// <summary>
        /// 机构ID
        /// </summary>
        [XmlElement("dback_inst_id")]
        public string DbackInstId { get; set; }

        /// <summary>
        /// 机构名称
        /// </summary>
        [XmlElement("dback_inst_name")]
        public string DbackInstName { get; set; }

        /// <summary>
        /// 统一支付资金条目ID
        /// </summary>
        [XmlElement("fid")]
        public string Fid { get; set; }

        /// <summary>
        /// 面向系统的资金工具接入类型
        /// </summary>
        [XmlElement("fund_access_type")]
        public long FundAccessType { get; set; }

        /// <summary>
        /// 资金账号。可以是支付宝主卡账号，子卡账号，银行卡号等等。
        /// </summary>
        [XmlElement("fund_account_no")]
        public string FundAccountNo { get; set; }

        /// <summary>
        /// 资金业务类型枚举
        /// </summary>
        [XmlElement("fund_biz_type")]
        public long FundBizType { get; set; }

        /// <summary>
        /// 资金明细创建时间
        /// </summary>
        [XmlElement("fund_create_time")]
        public string FundCreateTime { get; set; }

        /// <summary>
        /// 资金变动完成时间
        /// </summary>
        [XmlElement("fund_finish_time")]
        public string FundFinishTime { get; set; }

        /// <summary>
        /// 资金流向枚举
        /// </summary>
        [XmlElement("fund_in_out")]
        public long FundInOut { get; set; }

        /// <summary>
        /// 资金工具机构
        /// </summary>
        [XmlElement("fund_inst_id")]
        public string FundInstId { get; set; }

        /// <summary>
        /// 资金明细最后修改时间
        /// </summary>
        [XmlElement("fund_modify_time")]
        public string FundModifyTime { get; set; }

        /// <summary>
        /// 资金状态
        /// </summary>
        [XmlElement("fund_status")]
        public string FundStatus { get; set; }

        /// <summary>
        /// 该资金变动的资金工具是否为ownerCardNo所拥有
        /// </summary>
        [XmlElement("fund_tool_belong_to_crowner")]
        public bool FundToolBelongToCrowner { get; set; }

        /// <summary>
        /// fundToolBelongToCROwner为false时，该字段记录资金工具的实际拥有者
        /// </summary>
        [XmlElement("fund_tool_owner_card_no")]
        public string FundToolOwnerCardNo { get; set; }

        /// <summary>
        /// 面向用户的资金工具类型
        /// </summary>
        [XmlElement("fund_tool_type_for_crowner")]
        public string FundToolTypeForCrowner { get; set; }

        /// <summary>
        /// 面向系统的资金工具类型
        /// </summary>
        [XmlElement("fund_tool_type_for_system")]
        public string FundToolTypeForSystem { get; set; }

        /// <summary>
        /// 资金工具名字（中文），供外部直接展示用。
        /// </summary>
        [XmlElement("fund_tool_type_name")]
        public string FundToolTypeName { get; set; }

        /// <summary>
        /// 业务创建时间
        /// </summary>
        [XmlElement("gmt_biz_create")]
        public string GmtBizCreate { get; set; }

        /// <summary>
        /// 差错资金自服务入口开放类型，TAOBAO-对淘宝开放，ALIPAY-对支付宝站内开放
        /// </summary>
        [XmlElement("open_self_slip_type")]
        public string OpenSelfSlipType { get; set; }

        /// <summary>
        /// 导致该资金变动在业务上的另一方的卡别名
        /// </summary>
        [XmlElement("opposite_biz_card_alias")]
        public string OppositeBizCardAlias { get; set; }

        /// <summary>
        /// 导致该资金变动在业务上的另一方的卡别名。
        /// </summary>
        [XmlElement("opposite_biz_card_no")]
        public string OppositeBizCardNo { get; set; }

        /// <summary>
        /// 导致该资金变动在资金上的另一方的卡别名。
        /// </summary>
        [XmlElement("opposite_fund_card_no")]
        public string OppositeFundCardNo { get; set; }

        /// <summary>
        /// 外部请求号
        /// </summary>
        [XmlElement("out_request_no")]
        public string OutRequestNo { get; set; }

        /// <summary>
        /// 本方卡号
        /// </summary>
        [XmlElement("owner_card_no")]
        public string OwnerCardNo { get; set; }

        /// <summary>
        /// 银行卡退款进度状态
        /// </summary>
        [XmlElement("refund_bank_status")]
        public string RefundBankStatus { get; set; }

        /// <summary>
        /// 差错可提取金额，单位元
        /// </summary>
        [XmlElement("slip_amount")]
        public string SlipAmount { get; set; }

        /// <summary>
        /// 差错挂账id
        /// </summary>
        [XmlElement("slip_id")]
        public string SlipId { get; set; }

        /// <summary>
        /// 差错挂账资金申领状态，W-待申领，P-申领中，S-成功，F-失败
        /// </summary>
        [XmlElement("slip_status")]
        public string SlipStatus { get; set; }

        /// <summary>
        /// 预付子卡类型
        /// </summary>
        [XmlElement("sub_prepaid_card_type")]
        public string SubPrepaidCardType { get; set; }

        /// <summary>
        /// 统一支付ID
        /// </summary>
        [XmlElement("uid")]
        public string Uid { get; set; }
    }
}

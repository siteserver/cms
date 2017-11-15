using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayMarketingCardConsumeSyncModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayMarketingCardConsumeSyncModel : AopObject
    {
        /// <summary>
        /// 用户实际付的现金金额  1.针对预付卡面额的核销金额在use_benefit_list展现，作为权益金额  2.权益金额不叠加在该金额上
        /// </summary>
        [XmlElement("act_pay_amount")]
        public string ActPayAmount { get; set; }

        /// <summary>
        /// 卡信息（交易后的实际卡信息）
        /// </summary>
        [XmlElement("card_info")]
        public MerchantCard CardInfo { get; set; }

        /// <summary>
        /// 获取权益列表
        /// </summary>
        [XmlArray("gain_benefit_list")]
        [XmlArrayItem("benefit_info_detail")]
        public List<BenefitInfoDetail> GainBenefitList { get; set; }

        /// <summary>
        /// 备注信息，现有直接填写门店信息
        /// </summary>
        [XmlElement("memo")]
        public string Memo { get; set; }

        /// <summary>
        /// 门店编号
        /// </summary>
        [XmlElement("shop_code")]
        public string ShopCode { get; set; }

        /// <summary>
        /// 产生该笔交易时，用户出具的凭证类型  ALIPAY：支付宝电子卡  ENTITY：实体卡  OTHER：其他
        /// </summary>
        [XmlElement("swipe_cert_type")]
        public string SwipeCertType { get; set; }

        /// <summary>
        /// 支付宝业务卡号，开卡接口中返回获取
        /// </summary>
        [XmlElement("target_card_no")]
        public string TargetCardNo { get; set; }

        /// <summary>
        /// 卡号类型  BIZ_CARD：支付宝业务卡号
        /// </summary>
        [XmlElement("target_card_no_type")]
        public string TargetCardNoType { get; set; }

        /// <summary>
        /// 交易金额：本次交易的实际总金额（可认为标价金额）
        /// </summary>
        [XmlElement("trade_amount")]
        public string TradeAmount { get; set; }

        /// <summary>
        /// 交易名称  为空时根据交易类型提供默认名称
        /// </summary>
        [XmlElement("trade_name")]
        public string TradeName { get; set; }

        /// <summary>
        /// 支付宝交易号
        /// </summary>
        [XmlElement("trade_no")]
        public string TradeNo { get; set; }

        /// <summary>
        /// 线下交易时间（是用户付款的交易时间）  当交易时间晚于上次消费记录同步时间，则会发生卡信息变更
        /// </summary>
        [XmlElement("trade_time")]
        public string TradeTime { get; set; }

        /// <summary>
        /// 交易类型  开卡：OPEN  消费：TRADE  充值：DEPOSIT  退卡：RETURN
        /// </summary>
        [XmlElement("trade_type")]
        public string TradeType { get; set; }

        /// <summary>
        /// 实际消耗的权益
        /// </summary>
        [XmlArray("use_benefit_list")]
        [XmlArrayItem("benefit_info_detail")]
        public List<BenefitInfoDetail> UseBenefitList { get; set; }
    }
}

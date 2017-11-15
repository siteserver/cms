using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// VoucherDetailInfo Data Structure.
    /// </summary>
    [Serializable]
    public class VoucherDetailInfo : AopObject
    {
        /// <summary>
        /// 资产id
        /// </summary>
        [XmlElement("asset_id")]
        public string AssetId { get; set; }

        /// <summary>
        /// 有效期起
        /// </summary>
        [XmlElement("effect_time")]
        public string EffectTime { get; set; }

        /// <summary>
        /// 扩展字段信息，通过map存储的json串
        /// </summary>
        [XmlElement("ext_info")]
        public string ExtInfo { get; set; }

        /// <summary>
        /// 不可用时间段（只支持到天）
        /// </summary>
        [XmlElement("forbbiden_time")]
        public ForbbidenTime ForbbidenTime { get; set; }

        /// <summary>
        /// 券过期时间
        /// </summary>
        [XmlElement("invalid_time")]
        public string InvalidTime { get; set; }

        /// <summary>
        /// 单品id中间用“,”分隔
        /// </summary>
        [XmlElement("sku_codes")]
        public string SkuCodes { get; set; }

        /// <summary>
        /// 可用时段条款
        /// </summary>
        [XmlArray("time_rules")]
        [XmlArrayItem("use_time")]
        public List<UseTime> TimeRules { get; set; }

        /// <summary>
        /// 券使用条件
        /// </summary>
        [XmlElement("use_condition")]
        public string UseCondition { get; set; }

        /// <summary>
        /// 全资产描述
        /// </summary>
        [XmlElement("voucher_desc")]
        public string VoucherDesc { get; set; }

        /// <summary>
        /// 券类型(ALIPAY_FIX_VOUCHER:全场券；ALIPAY_ITEM_VOUCHER：单品券,KOUBEI_BUY_EXCHANGE_VOUCHER:兑换券)
        /// </summary>
        [XmlElement("voucher_type")]
        public string VoucherType { get; set; }
    }
}

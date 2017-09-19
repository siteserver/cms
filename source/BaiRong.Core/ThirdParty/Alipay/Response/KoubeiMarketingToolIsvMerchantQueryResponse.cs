using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// KoubeiMarketingToolIsvMerchantQueryResponse.
    /// </summary>
    public class KoubeiMarketingToolIsvMerchantQueryResponse : AopResponse
    {
        /// <summary>
        /// 商户信息列表
        /// </summary>
        [XmlArray("merchant_infos")]
        [XmlArrayItem("isv_merchant_info")]
        public List<IsvMerchantInfo> MerchantInfos { get; set; }

        /// <summary>
        /// 门店总量
        /// </summary>
        [XmlElement("shop_count")]
        public string ShopCount { get; set; }

        /// <summary>
        /// 门店详情列表信息
        /// </summary>
        [XmlArray("shop_summary_infos")]
        [XmlArrayItem("shop_summary_info")]
        public List<ShopSummaryInfo> ShopSummaryInfos { get; set; }
    }
}

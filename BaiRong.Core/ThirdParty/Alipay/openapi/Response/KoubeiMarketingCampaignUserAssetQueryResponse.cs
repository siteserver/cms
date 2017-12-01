using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// KoubeiMarketingCampaignUserAssetQueryResponse.
    /// </summary>
    public class KoubeiMarketingCampaignUserAssetQueryResponse : AopResponse
    {
        /// <summary>
        /// 券资产详情信息
        /// </summary>
        [XmlArray("voucher_asset_list")]
        [XmlArrayItem("voucher_detail_info")]
        public List<VoucherDetailInfo> VoucherAssetList { get; set; }

        /// <summary>
        /// 券资产数量
        /// </summary>
        [XmlElement("voucher_asset_num")]
        public long VoucherAssetNum { get; set; }
    }
}

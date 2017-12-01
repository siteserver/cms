using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiMarketingCampaignAssetDetailQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiMarketingCampaignAssetDetailQueryModel : AopObject
    {
        /// <summary>
        /// 用户资产id，配合《用户口碑优惠资产查询接口》使用，返回券资产信息列表中的asset_id则为传递的入参值。
        /// </summary>
        [XmlElement("asset_id")]
        public string AssetId { get; set; }

        /// <summary>
        /// 资产类型(VOUCHER:券资产)  配合《用户口碑优惠资产查询接口》使用，返回券资产详情信息，则对应VOUCHER类型
        /// </summary>
        [XmlElement("asset_type")]
        public string AssetType { get; set; }
    }
}

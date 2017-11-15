using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayOfflineMarketShopSummaryBatchqueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayOfflineMarketShopSummaryBatchqueryModel : AopObject
    {
        /// <summary>
        /// 表示接口业务的调用方身份：ISV、 服务商身份标识。传入ISV代表系统集成商身份。传入PROVIDER代表服务商。
        /// </summary>
        [XmlElement("op_role")]
        public string OpRole { get; set; }

        /// <summary>
        /// 页码，留空标示第一页，默认 20个结果为一页
        /// </summary>
        [XmlElement("page_no")]
        public long PageNo { get; set; }

        /// <summary>
        /// 每页记录数，默认20，最大 100
        /// </summary>
        [XmlElement("page_size")]
        public long PageSize { get; set; }

        /// <summary>
        /// 门店数据查询类型，根据类型可以返回指定的门店数据，目前支持的类型如下：  BRAND_RELATION ： 品牌商关联店铺  MALL_SELF ：MALL自己的门店  MALL_RELATION：MALL关联下的门店  MERCHANT_SELF:商户自己的门店  KB_PROMOTER：口碑客推广者
        /// </summary>
        [XmlElement("query_type")]
        public string QueryType { get; set; }

        /// <summary>
        /// query_type查询类型下所关联的商户PID
        /// </summary>
        [XmlElement("related_partner_id")]
        public string RelatedPartnerId { get; set; }

        /// <summary>
        /// 门店ID
        /// </summary>
        [XmlElement("shop_id")]
        public string ShopId { get; set; }

        /// <summary>
        /// 门店状态，传入多个状态，多个状态使用英文逗号隔开，例如：PAUSED,OPEN  店铺状态：OPEN（营业）、PAUSED（暂停）、INIT（初始）、FREEZE（冻结）、CLOSED（关店）
        /// </summary>
        [XmlElement("shop_status")]
        public string ShopStatus { get; set; }
    }
}

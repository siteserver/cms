using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// AntMerchantExpandMerchantStorelistQueryResponse.
    /// </summary>
    public class AntMerchantExpandMerchantStorelistQueryResponse : AopResponse
    {
        /// <summary>
        /// 商户门店列表
        /// </summary>
        [XmlArray("merchant_stores")]
        [XmlArrayItem("shop_query_info")]
        public List<ShopQueryInfo> MerchantStores { get; set; }

        /// <summary>
        /// 当前页码,页码从1开始
        /// </summary>
        [XmlElement("page_num")]
        public long PageNum { get; set; }

        /// <summary>
        /// 每页条数
        /// </summary>
        [XmlElement("page_size")]
        public long PageSize { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        [XmlElement("total_pages")]
        public long TotalPages { get; set; }

        /// <summary>
        /// 总条数
        /// </summary>
        [XmlElement("total_size")]
        public long TotalSize { get; set; }
    }
}

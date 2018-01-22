using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayOfflineMarketShopPublicBindResponse.
    /// </summary>
    public class AlipayOfflineMarketShopPublicBindResponse : AopResponse
    {
        /// <summary>
        /// 绑定失败的店铺ID列表
        /// </summary>
        [XmlArray("error_binding_shop_ids")]
        [XmlArrayItem("string")]
        public List<string> ErrorBindingShopIds { get; set; }

        /// <summary>
        /// 数字类型，标示总的绑定错误总数
        /// </summary>
        [XmlElement("total_error")]
        public long TotalError { get; set; }

        /// <summary>
        /// 绑定店铺总成功数
        /// </summary>
        [XmlElement("total_success")]
        public long TotalSuccess { get; set; }
    }
}

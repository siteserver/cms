using System;
using System.Xml.Serialization;
using System.Collections.Generic;
using Aop.Api.Domain;

namespace Aop.Api.Response
{
    /// <summary>
    /// ZhimaCreditWatchlistiiGetResponse.
    /// </summary>
    public class ZhimaCreditWatchlistiiGetResponse : AopResponse
    {
        /// <summary>
        /// 芝麻信用对于每一次请求返回的业务号。后续可以通过此业务号进行对账
        /// </summary>
        [XmlElement("biz_no")]
        public string BizNo { get; set; }

        /// <summary>
        /// 所查询的某个用户的行业关注名单列表。当is_matched为true时会返回该参数。
        /// </summary>
        [XmlArray("details")]
        [XmlArrayItem("zm_watch_list_detail")]
        public List<ZmWatchListDetail> Details { get; set; }

        /// <summary>
        /// true=命中 在关注名单中  false=未命中
        /// </summary>
        [XmlElement("is_matched")]
        public bool IsMatched { get; set; }
    }
}

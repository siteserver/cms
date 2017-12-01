using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// KoubeiMarketingDataBizadviserMyddsreportQueryResponse.
    /// </summary>
    public class KoubeiMarketingDataBizadviserMyddsreportQueryResponse : AopResponse
    {
        /// <summary>
        /// result是一个所有结果集合的json串。  result结果集是一个json格式数组， 是门店热力图信息  lng: 位置经度 lat:位置维度 cnt：会员数量
        /// </summary>
        [XmlElement("result")]
        public string Result { get; set; }
    }
}

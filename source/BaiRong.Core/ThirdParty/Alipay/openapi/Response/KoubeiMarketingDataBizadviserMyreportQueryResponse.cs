using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// KoubeiMarketingDataBizadviserMyreportQueryResponse.
    /// </summary>
    public class KoubeiMarketingDataBizadviserMyreportQueryResponse : AopResponse
    {
        /// <summary>
        /// 不同uniq_key 返回不同参数，详细请看：https://doc.open.alipay.com/docs/doc.htm?spm=0.0.0.0.1Zq3by&docType=1&articleId=107061&previewCode=F30AA2D06B7CC0817AC7221A8CE288A4
        /// </summary>
        [XmlElement("result")]
        public string Result { get; set; }
    }
}

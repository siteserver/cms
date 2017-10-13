using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// KoubeiMarketingDataIndicatorQueryResponse.
    /// </summary>
    public class KoubeiMarketingDataIndicatorQueryResponse : AopResponse
    {
        /// <summary>
        /// JSON格式数组，每个对象表示一个门店某个具体日期的指标信息，KEY为指标代码，VALUE为该指标对应的值,各biz_type入参以及返回值的详细信息参见<a href="https://doc.open.alipay.com/docs/doc.htm?spm=a219a.7629140.0.0.1AZ2QH&treeId=193&articleId=106028&docType=1">快速接入</a>
        /// </summary>
        [XmlElement("indicator_infos")]
        public string IndicatorInfos { get; set; }

        /// <summary>
        /// 总条目数,供计算分页信息使用
        /// </summary>
        [XmlElement("total_size")]
        public string TotalSize { get; set; }
    }
}

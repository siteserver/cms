using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// KoubeiMarketingCampaignTagsQueryResponse.
    /// </summary>
    public class KoubeiMarketingCampaignTagsQueryResponse : AopResponse
    {
        /// <summary>
        /// 查询成功时返回人群标签信息查询失败时为空  code:表示标签code  name:表示标签名称  valueRange：表示标签的取值范围  value:表示标签具体取值  label:描述信息  标签相关的详细信息参见附件。<a href="http://aopsdkdownload.cn-hangzhou.alipay-pub.aliyun-inc.com/doc/tags%26usecase.zip">标签信息</a>
        /// </summary>
        [XmlElement("tags")]
        public string Tags { get; set; }
    }
}

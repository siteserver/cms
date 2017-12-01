using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// ZhimaCreditWatchlistBriefGetResponse.
    /// </summary>
    public class ZhimaCreditWatchlistBriefGetResponse : AopResponse
    {
        /// <summary>
        /// 唯一标示每一次接口调用
        /// </summary>
        [XmlElement("biz_no")]
        public string BizNo { get; set; }

        /// <summary>
        /// 输入用户返回结果：  0 未命中逾期名单  1 命中一类名单，例如用户有一周以内的轻微逾期  2 命中二类名单，例如用户有一周以上中等逾期  3 命中三类名单，例如用户有一个月以上的严重逾期  N/A 无法评估该用户逾期状况，例如未获得用户授权。
        /// </summary>
        [XmlElement("level")]
        public string Level { get; set; }
    }
}

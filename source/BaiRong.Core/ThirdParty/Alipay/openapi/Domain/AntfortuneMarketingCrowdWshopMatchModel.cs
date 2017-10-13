using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AntfortuneMarketingCrowdWshopMatchModel Data Structure.
    /// </summary>
    [Serializable]
    public class AntfortuneMarketingCrowdWshopMatchModel : AopObject
    {
        /// <summary>
        /// 财富号机构自建人群id，应用于财富号机构人群匹配。财富号合作机构通过财富号后台创建人群后获得人群id
        /// </summary>
        [XmlElement("crowd_id")]
        public string CrowdId { get; set; }

        /// <summary>
        /// 蚂蚁统一会员ID，通过alipay.user.info.share接口获取
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}

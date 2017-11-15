using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiMarketingCampaignCrowdCountModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiMarketingCampaignCrowdCountModel : AopObject
    {
        /// <summary>
        /// 圈人的条件  op:表示操作符，目前支持EQ相等,GT大于,GTEQ大于等于,LT小于,LTEQ小于等于,NEQ不等,LIKE模糊匹配,IN在枚举范围内,NOTIN不在枚举范围内,BETWEEN范围比较,LEFTDAYS几天以内,RIGHTDAYS几天以外,LOCATE地理位置比较,LBS地图位置数据  tagCode:标签code，详细标签code参见附件。<a href="http://aopsdkdownload.cn-hangzhou.alipay-pub.aliyun-inc.com/doc/tags%26usecase.zip">标签信息</a>  value:标签值
        /// </summary>
        [XmlElement("conditions")]
        public string Conditions { get; set; }

        /// <summary>
        /// crowd_group_id和conditions不能同时为空  如果crowd_group_id不为空则根据crowd_group_id查询人群分组的信息进行统计，否则以conditions的内容为过滤条件进行统计，如果crowd_group_id和conditions都不为空则优先使用conditions的条件
        /// </summary>
        [XmlElement("crowd_group_id")]
        public string CrowdGroupId { get; set; }

        /// <summary>
        /// 画像分析的维度，目前支持:["pam_age","pam_gender","pam_constellation","pam_hometown_code","pam_city_code","pam_occupation","pam_consume_level","pam_have_baby"]，以koubei.marketing.campaign.tags.query接口返回的dimensions为准，各个维度标签的详细信息参见附件，<a href="http://aopsdkdownload.cn-hangzhou.alipay-pub.aliyun-inc.com/doc/tags%26usecase.zip">标签信息</a>
        /// </summary>
        [XmlElement("dimensions")]
        public string Dimensions { get; set; }
    }
}

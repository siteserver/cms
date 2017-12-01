using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayMarketingCampaignRuleCrowdCreateModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayMarketingCampaignRuleCrowdCreateModel : AopObject
    {
        /// <summary>
        /// tagCode：标签别名，指定标签类型，如城市、年龄等，如pubsrv_city_code为城市标签，创建时必填，可传入多组标签  op：操作码，EQ（等于）、IN（包含），tagCode与op绑定，以查询出来的标签操作符确定该用EQ还是IN  value：tagCode对应选中的值，如：440100为城市地区代码，可支持多个     具体参数参考如下：  1）消费能力：pubsrv_consume_level 操作符：(EQ、IN) 枚举值：高、中、低  2）性别：pubsrv_gender 操作符：(EQ)  枚举值：1男，2女，0未知  3）年龄区间：pubsrv_age操作符：(EQ、IN) 枚举值见后面年龄区间标签枚举定义  4）职业：pubsrv_occupation操作符：(EQ) 枚举值：白领、大学生、未知    5）常驻城市CODE：pubsrv_city_code操作符：(EQ) 行政区划代码 样例数据：440100   6)住房类型：pubsrv_house_type操作符：(EQ) 枚举值：rent 租房,home 自有住房，other 其他  7)婚姻状态：pubsrv_is_married操作符：(EQ) 枚举值：0 否，1 是  8)是否有小孩：pubsrv_have_baby操作符：(EQ) 枚举值：0 否，1 是  9)是否实名认证：pubsrv_is_realname操作符：(EQ) 枚举值：0 否，1 是，2 未知  10)是否有车：pubsrv_have_auto操作符：(EQ) 枚举值：0 否，1 是，2 未知  11)达人偏好：pubsrv_preference操作符：(IN) 多值列，枚举值：travel=旅游;video=影视;game=游戏;music=音乐;photography=摄影;pet=宠物;sports=运动;digital=数码    * 年龄区间标签枚举 枚举值: 1-11  user_age <=17 then 1  [18,20] then 2  [21,25] then 3  [26,30] then 4  [31,35] then 5  [36,40] then 6  [41,45] then 7  [46,50] then 8  [51,55] then 9  [56,60] then 10  user_age >60 then 11
        /// </summary>
        [XmlElement("mdatacrowdsource")]
        public string Mdatacrowdsource { get; set; }

        /// <summary>
        /// 签约商户下属机构唯一编号
        /// </summary>
        [XmlElement("mpid")]
        public string Mpid { get; set; }

        /// <summary>
        /// 圈人规则描述，说明规则用途
        /// </summary>
        [XmlElement("ruledesc")]
        public string Ruledesc { get; set; }
    }
}

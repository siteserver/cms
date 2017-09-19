using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiMarketingDataSmartactivityConfigModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiMarketingDataSmartactivityConfigModel : AopObject
    {
        /// <summary>
        /// 诊断结果CODE，目前有如下四个值  TRADE_RATE 流失会员占比高  USER_COUNT 会员数量少  REPAY_RATE 复购率低  SUPER_ITEM  建议打造单品爆款(适用于菜品营销)  注意：当入参的诊断码为SUPER_ITEM表示菜品营销的诊断时，下面的返回结果中如果有多个菜品时，下面各活动配置的参数使用竖线|来将各个值分隔。例如：菜品名称item_name的值：剁椒鱼头|鱼香茄子，使用横线-表示为空的数据，例如：领券门槛min_cost的值如果没有则返回  -|-,返回和item_id数量一致的横线
        /// </summary>
        [XmlElement("diagnose_code")]
        public string DiagnoseCode { get; set; }
    }
}

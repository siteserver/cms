using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// KoubeiMarketingDataTradeHabbitQueryResponse.
    /// </summary>
    public class KoubeiMarketingDataTradeHabbitQueryResponse : AopResponse
    {
        /// <summary>
        /// biz_date: 业务日期  格式:yyyyMMdd       partner_industry_type:商户行业标识（轻餐or正餐）       shop_id: 门店id       shop_name: 门店名称       trade_again_ratio_7d: （近7天）复购率       trade_again_ratio_active_7d: （近7天）复购率（口碑活跃用户）       trade_user_cnt_7d: （近7天）交易会员数       trade_user_cnt_active_7d: （近7天）交易活跃会员数（口碑活跃用户贡献）       zdj_order_amt_7d: （近7天）桌单价（订单金额）(单位:分)       zdj_service_amt_7d: （近7天）桌单价（实收金额）(单位:分)       zdj_service_amt_distribution_7d: （近7天）桌单价（实收金额）段分布（示例：0-15:0.2799,15-20:0.1775,20-25:0.1058,25-30:0.0956,30-35:0.0648,35-40:0.0751,40-45:0.0444,45-50:0.0137,50-55:0.0171,55及以上:0.1263 意思是消费0-15元的比例占消费总人数的0.2799）  注意：出参中还包含30天/60天/90天的类似指标，篇幅有限不一一列举
        /// </summary>
        [XmlElement("trade_habit_info")]
        public string TradeHabitInfo { get; set; }
    }
}

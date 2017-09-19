using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// DiagnoseResult Data Structure.
    /// </summary>
    [Serializable]
    public class DiagnoseResult : AopObject
    {
        /// <summary>
        /// 提示文案业务参数,JSON对象形式返回，JSON的KEY包含tradeCycle，userRate，industryRate，repayRate调用方根据诊断CODE分别给出不同的诊断文案，例如：  TRADE_RATE 流失会员占比高 您当前${tradeCycle}(90)天未到店消费会员占总会员${userRate}(40%)，落后同行${industryRate}(60%)的商家。      USER_COUNT 会员数量少 您当前店均会员量较少，落后同行${industryRate}(60%)的商家。        REPAY_RATE 复购率低 您当前${tradeCycle}(60)天会员回头率为${repayRate}(30%)，落后于同行${industryRate}(60%)的商家。
        /// </summary>
        [XmlElement("biz_data")]
        public string BizData { get; set; }

        /// <summary>
        /// 诊断结果CODE，目前有如下三个值  TRADE_RATE 流失会员占比高  USER_COUNT 会员数量少  REPAY_RATE 复购率低
        /// </summary>
        [XmlElement("diagnose_code")]
        public string DiagnoseCode { get; set; }
    }
}

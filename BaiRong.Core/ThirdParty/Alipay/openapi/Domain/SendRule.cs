using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// SendRule Data Structure.
    /// </summary>
    [Serializable]
    public class SendRule : AopObject
    {
        /// <summary>
        /// 是否允许重复发奖：  true代表允许，false代表不允许  默认不设置，表明用户领取券后如果没有核销则不允许再次领取券  如果设置为true，表明如果用户领取券后没有核销，还可以继续领取该券
        /// </summary>
        [XmlElement("allow_repeat_send")]
        public string AllowRepeatSend { get; set; }

        /// <summary>
        /// 发券最低消费金额，单位元  活动类型为消费送且不是消费送礼包时设置  多营销工具之间不允许设置重复值
        /// </summary>
        [XmlElement("min_cost")]
        public string MinCost { get; set; }

        /// <summary>
        /// 券的预算数量（仅对口令送随机抽奖有效，即当活动类型为GUESS_SEND，且营销工具PromoTool的个数大于1时，此字段必填，其余情况此字段必为空）
        /// </summary>
        [XmlElement("send_budget")]
        public string SendBudget { get; set; }

        /// <summary>
        /// 发券数目  最少发1张券，最多发5张券
        /// </summary>
        [XmlElement("send_num")]
        public string SendNum { get; set; }
    }
}

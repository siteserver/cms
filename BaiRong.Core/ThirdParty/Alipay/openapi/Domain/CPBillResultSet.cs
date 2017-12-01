using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// CPBillResultSet Data Structure.
    /// </summary>
    [Serializable]
    public class CPBillResultSet : AopObject
    {
        /// <summary>
        /// 账期
        /// </summary>
        [XmlElement("acct_period")]
        public string AcctPeriod { get; set; }

        /// <summary>
        /// 应收金额，单位为元，精确到小数点后两位，取值范围[0.01,100000000]
        /// </summary>
        [XmlElement("bill_entry_amount")]
        public string BillEntryAmount { get; set; }

        /// <summary>
        /// 物业费账单应收明细条目ID
        /// </summary>
        [XmlElement("bill_entry_id")]
        public string BillEntryId { get; set; }

        /// <summary>
        /// 费用类型
        /// </summary>
        [XmlElement("cost_type")]
        public string CostType { get; set; }

        /// <summary>
        /// 缴费截止日期
        /// </summary>
        [XmlElement("deadline")]
        public string Deadline { get; set; }

        /// <summary>
        /// 物业系统端房屋编号
        /// </summary>
        [XmlElement("out_room_id")]
        public string OutRoomId { get; set; }

        /// <summary>
        /// 出账日期
        /// </summary>
        [XmlElement("release_day")]
        public string ReleaseDay { get; set; }

        /// <summary>
        /// 房屋门牌地址
        /// </summary>
        [XmlElement("room_address")]
        public string RoomAddress { get; set; }

        /// <summary>
        /// 账单条目当前状态，状态值：  FINISH_PAYMENT - 用户完成支付和销账  UNDER_PAYMENT - 账单锁定待用户完成支付  WAIT_PAYMENT - 待缴且未过缴费截止日期  OUT_OF_DATE - 未支付且已过缴费截止日期
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }
    }
}

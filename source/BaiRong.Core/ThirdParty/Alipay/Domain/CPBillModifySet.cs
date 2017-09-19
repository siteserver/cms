using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// CPBillModifySet Data Structure.
    /// </summary>
    [Serializable]
    public class CPBillModifySet : AopObject
    {
        /// <summary>
        /// 若账期需修改，则传入。账期用于缴费明细页归类和展示，可以使用不超过16个字符的有业务含义的字符串。
        /// </summary>
        [XmlElement("acct_period")]
        public string AcctPeriod { get; set; }

        /// <summary>
        /// 若应收金额需修改，则传入，单位为元，精确到小数点后两位，取值范围[0.01,100000000]
        /// </summary>
        [XmlElement("bill_entry_amount")]
        public string BillEntryAmount { get; set; }

        /// <summary>
        /// 待修改的物业费账单应收明细条目ID
        /// </summary>
        [XmlElement("bill_entry_id")]
        public string BillEntryId { get; set; }

        /// <summary>
        /// 若费用类型需修改，则传入
        /// </summary>
        [XmlElement("cost_type")]
        public string CostType { get; set; }

        /// <summary>
        /// 若缴费截止日期需修改，则传入。格式固定为YYYYMMDD
        /// </summary>
        [XmlElement("deadline")]
        public string Deadline { get; set; }

        /// <summary>
        /// 若出账日期需修改，则传入，格式固定为YYYYMMDD
        /// </summary>
        [XmlElement("release_day")]
        public string ReleaseDay { get; set; }

        /// <summary>
        /// 若房屋门牌地址需要修改，则传入该值
        /// </summary>
        [XmlElement("room_address")]
        public string RoomAddress { get; set; }
    }
}

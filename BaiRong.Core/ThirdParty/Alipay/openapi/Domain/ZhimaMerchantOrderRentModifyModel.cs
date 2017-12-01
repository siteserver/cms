using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ZhimaMerchantOrderRentModifyModel Data Structure.
    /// </summary>
    [Serializable]
    public class ZhimaMerchantOrderRentModifyModel : AopObject
    {
        /// <summary>
        /// 芝麻借还订单的开始借用时间，格式：yyyy-mm-dd hh:MM:ss   如果同时传入另一参数:应归还时间expiry_time，则传入的开始借用时间不能晚于传入的应归还时间，如果没有传入应归还时间，则传入的开始借用时间不能晚于原有应归还时间。  borrow_time 与 expiry_time 须至少传入一个，可同时传入。
        /// </summary>
        [XmlElement("borrow_time")]
        public string BorrowTime { get; set; }

        /// <summary>
        /// 芝麻借还订单的应归还时间(到期时间)，格式：yyyy-mm-dd hh:MM:ss   传入的应归还时间不能早于原有应归还时间。  borrow_time 与 expiry_time 须至少传入一个，可同时传入。
        /// </summary>
        [XmlElement("expiry_time")]
        public string ExpiryTime { get; set; }

        /// <summary>
        /// 信用借还订单号,该订单号在订单创建时由信用借还产品产生,并通过订单创建接口的返回结果返回给调用者
        /// </summary>
        [XmlElement("order_no")]
        public string OrderNo { get; set; }

        /// <summary>
        /// 信用借还的产品码,是固定值:w1010100000000002858
        /// </summary>
        [XmlElement("product_code")]
        public string ProductCode { get; set; }
    }
}

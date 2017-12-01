using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEcoMycarMaintainShopDeleteModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEcoMycarMaintainShopDeleteModel : AopObject
    {
        /// <summary>
        /// 外部门店编号（与shop_id二选一，不能都为空）
        /// </summary>
        [XmlElement("out_shop_id")]
        public string OutShopId { get; set; }

        /// <summary>
        /// 车主平台门店编号（与out_shop_id二选一，不能都为空）
        /// </summary>
        [XmlElement("shop_id")]
        public long ShopId { get; set; }
    }
}

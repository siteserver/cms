using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiSalesKbassetStuffPurchaseorderQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiSalesKbassetStuffPurchaseorderQueryModel : AopObject
    {
        /// <summary>
        /// 每页大小：最小1，最大100
        /// </summary>
        [XmlElement("page_size")]
        public string PageSize { get; set; }
    }
}

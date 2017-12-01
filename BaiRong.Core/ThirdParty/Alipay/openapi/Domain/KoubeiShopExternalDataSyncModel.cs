using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// KoubeiShopExternalDataSyncModel Data Structure.
    /// </summary>
    [Serializable]
    public class KoubeiShopExternalDataSyncModel : AopObject
    {
        /// <summary>
        /// 操作类型：Bind:建立口碑门店和饿了么外卖关系  unBind：解除口碑门店和饿了么外卖关系  sync：同步门店营业时间、营业状态、店铺状态
        /// </summary>
        [XmlElement("action")]
        public string Action { get; set; }

        /// <summary>
        /// shop_status:OPEN（生效）||CLOSE（失效） ，饿了么签约状态  business_time：08：00-11：30,13：00-20：30，营业时间，多个逗号分隔  business_status：OPEN（营业）||CLOSE（歇业）  饿了么营业状态。
        /// </summary>
        [XmlElement("content")]
        public string Content { get; set; }

        /// <summary>
        /// 数据来源。固定值:elm
        /// </summary>
        [XmlElement("data_source")]
        public string DataSource { get; set; }

        /// <summary>
        /// 数据版本（时间戳）。用于判断请求是否乱序。
        /// </summary>
        [XmlElement("data_version")]
        public long DataVersion { get; set; }

        /// <summary>
        /// 外部的门店id
        /// </summary>
        [XmlElement("external_shop_id")]
        public string ExternalShopId { get; set; }

        /// <summary>
        /// 口碑店铺Id
        /// </summary>
        [XmlElement("shop_id")]
        public string ShopId { get; set; }
    }
}

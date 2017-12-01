using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEcoMycarMaintainServiceproductUpdateModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEcoMycarMaintainServiceproductUpdateModel : AopObject
    {
        /// <summary>
        /// 请求操作（INSERT：新增；MODIFY：修改；DELETE：删除；QUERY：查询）
        /// </summary>
        [XmlElement("operation_type")]
        public string OperationType { get; set; }

        /// <summary>
        /// 外部服务商品标示，ISV保证唯一性。ISV同一门店同一服务项同一产品名称，只能配置一个商品。（存在同一服务项类目对应多个产品情况，5座普通洗车、5座SUV洗车）
        /// </summary>
        [XmlElement("out_product_id")]
        public string OutProductId { get; set; }

        /// <summary>
        /// 查询删除：否  新增修改：必填
        /// </summary>
        [XmlElement("shop_product")]
        public MaitainShopProduct ShopProduct { get; set; }
    }
}

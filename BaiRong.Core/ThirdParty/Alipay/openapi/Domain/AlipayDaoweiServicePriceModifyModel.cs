using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayDaoweiServicePriceModifyModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayDaoweiServicePriceModifyModel : AopObject
    {
        /// <summary>
        /// 外部服务id,商家自己维护的唯一标识,用于确定商家的某个服务.仅支持数字,字母和下划线
        /// </summary>
        [XmlElement("out_service_id")]
        public string OutServiceId { get; set; }

        /// <summary>
        /// 价格维度类型，可选值:string；json。string表示unit_price的类型是一维价格，如果是json表示多维价格
        /// </summary>
        [XmlElement("price_dim_type")]
        public string PriceDimType { get; set; }

        /// <summary>
        /// 单价，单位为元，根据price_dim_type的值决定如果是一维价格直接使用字符串，比如："30.5"；如果是多维，需要跟SKU结合进行定价，SKU通过alipay.daowei.service.modify接口在创建服务的时候创建。例子:  [{out_sku_id: 1, price: 50.5},   {out_sku_id: 2, price: 60.5},  ]  out_sku_id是在sku中定义的外部商品库存单位信息ID,该配置表示out_sku_id为1的时候对应的价格是50.5，out_sku_id为2的时候对应的价格是60.5
        /// </summary>
        [XmlElement("unit_price")]
        public string UnitPrice { get; set; }
    }
}

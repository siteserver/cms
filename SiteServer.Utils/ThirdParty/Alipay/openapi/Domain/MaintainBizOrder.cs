using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// MaintainBizOrder Data Structure.
    /// </summary>
    [Serializable]
    public class MaintainBizOrder : AopObject
    {
        /// <summary>
        /// 原始金额。服务对应原始价格。对应划掉的服务价格。金额单位(元)，保留两位小数。
        /// </summary>
        [XmlElement("origin_cost")]
        public string OriginCost { get; set; }

        /// <summary>
        /// 外部服务商品编码。下单时记录ISV服务商品唯一标示。用于区分不同商品
        /// </summary>
        [XmlElement("out_product_id")]
        public string OutProductId { get; set; }

        /// <summary>
        /// 销售金额。服务真实售卖单价。金额单位(元)，保留两位小数。
        /// </summary>
        [XmlElement("real_cost")]
        public string RealCost { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        [XmlElement("sale_num")]
        public long SaleNum { get; set; }

        /// <summary>
        /// 车主平台服务类目编号。    洗车类：  4:普通洗车-5座  5:普通洗车-7座  6:精细洗车-5座  7:精细洗车-7座  保养类：  12:更换机油、机滤  4S保养类：  401001:更换机油(4S)    401001:更换机油滤清器(4S)    401001:更换油塞密封垫片(4S)  401001:更换空气滤清器(4S)    401001:更换空调滤清器(4S)    401001:更换燃油滤清器(4S)    401001:更换制动液(4S)    401001:更换火花塞(4S)    401001:更换手动变速器油(4S)    401001:更换自动变速箱油(4S)  401001:更换冷却液(4S)    401001:更换转向油(4S)  401001:更换助力油(4S)  401001:发送机正时套件(4S)    401001:碳罐(4S)    401001:轮胎换位(4S)    401001:离合器油(4S)    401001:传动皮带套件(4S)    401001:冷媒（4S）   4S养护类：  402001:清洗节气门(4S)  402001:清洗喷油嘴(4S)  402001:清洗三元催化(4S)  402001:清洗进气道(4S)  402001:添加新车保护剂(4S)  402001:发动机润滑系统清洗(4S)  402001:发动机内部保护(4S)
        /// </summary>
        [XmlElement("service_category_id")]
        public long ServiceCategoryId { get; set; }
    }
}

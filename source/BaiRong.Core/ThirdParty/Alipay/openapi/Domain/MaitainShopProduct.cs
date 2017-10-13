using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// MaitainShopProduct Data Structure.
    /// </summary>
    [Serializable]
    public class MaitainShopProduct : AopObject
    {
        /// <summary>
        /// 销售价格（元），服务售卖价格，展示给用户。无优惠时的下单支付金额。 新增：必填； 修改：可空
        /// </summary>
        [XmlElement("off_price")]
        public string OffPrice { get; set; }

        /// <summary>
        /// 原始价格（元），服务商品原始价格，可以和销售价格相同。 新增：必填； 修改：可空
        /// </summary>
        [XmlElement("orig_price")]
        public string OrigPrice { get; set; }

        /// <summary>
        /// ISV 优惠活动ID（由ISV自行生成，保证其唯一）  新增：可空； 修改：可空  (out_privilege_id、privilege_start_time、privilege_close_time、privilege_tags、privilege_price必须同时存在)
        /// </summary>
        [XmlElement("out_privilege_id")]
        public string OutPrivilegeId { get; set; }

        /// <summary>
        /// 外部门店编号。门店创建时上传的ISV自己的门店唯一标示  新增：必填； 修改：可空
        /// </summary>
        [XmlElement("out_shop_id")]
        public string OutShopId { get; set; }

        /// <summary>
        /// 优惠截止时间。格式"yyyy-MM-dd HH:mm:ss"  优惠有效时间，超过时间不再显示优惠价，下单使用销售价格。  新增：可空； 修改：可空  (out_privilege_id、privilege_start_time、privilege_close_time、privilege_tags、privilege_price必须同时存在)
        /// </summary>
        [XmlElement("privilege_close_time")]
        public string PrivilegeCloseTime { get; set; }

        /// <summary>
        /// 优惠价格（元），ISV上传优惠价格，上传后立刻生效。有效期间为下单支付金额。 新增：可空； 修改：可空 (out_privilege_id、privilege_start_time、privilege_close_time、privilege_tags、privilege_price必须同时存在)
        /// </summary>
        [XmlElement("privilege_price")]
        public string PrivilegePrice { get; set; }

        /// <summary>
        /// 优惠截止时间。格式"yyyy-MM-dd HH:mm:ss"  优惠有效时间，超过时间不再显示优惠价，下单使用销售价格。  新增：可空； 修改：可空  (out_privilege_id、privilege_start_time、privilege_close_time、privilege_tags、privilege_price必须同时存在)
        /// </summary>
        [XmlElement("privilege_start_time")]
        public string PrivilegeStartTime { get; set; }

        /// <summary>
        /// 优惠标签，描述优惠的简要描述。门店详情页服务商品透出优惠标签。支持多个标签，多个逗号分隔。每个不超过10个字符。最多不超过10个标签。  新增：可空； 修改：可空  (out_privilege_id、privilege_start_time、privilege_close_time、privilege_tags、privilege_price必须同时存在)
        /// </summary>
        [XmlElement("privilege_tags")]
        public string PrivilegeTags { get; set; }

        /// <summary>
        /// 产品描述，ISV对服务项的描述。在门店详情页服务产品名下透出描述。只允许对服务产品本身进行描述，不能透出ISV名称等敏感数据。  新增：必填； 修改：可空
        /// </summary>
        [XmlElement("product_desc")]
        public string ProductDesc { get; set; }

        /// <summary>
        /// 产品名称，门店详情页展示的服务商品名称。（同一服务项可以配置5座普通洗车、5座SUV洗车）。  新增：必填； 修改：可空
        /// </summary>
        [XmlElement("product_name")]
        public string ProductName { get; set; }

        /// <summary>
        /// 车主平台服务类目编号。    新增：必填； 修改：可空  洗车类：  4:普通洗车-5座  5:普通洗车-7座  6:精细洗车-5座  7:精细洗车-7座  保养类：  12:更换机油、机滤  4S保养类：  401001:更换机油(4S)    401001:更换机油滤清器(4S)    401001:更换油塞密封垫片(4S)  401001:更换空气滤清器(4S)    401001:更换空调滤清器(4S)    401001:更换燃油滤清器(4S)    401001:更换制动液(4S)    401001:更换火花塞(4S)    401001:更换手动变速器油(4S)    401001:更换自动变速箱油(4S)  401001:更换冷却液(4S)    401001:更换转向油(4S)  401001:更换助力油(4S)  401001:发送机正时套件(4S)    401001:碳罐(4S)    401001:轮胎换位(4S)    401001:离合器油(4S)    401001:传动皮带套件(4S)    401001:冷媒（4S）   4S养护类：  402001:清洗节气门(4S)  402001:清洗喷油嘴(4S)  402001:清洗三元催化(4S)  402001:清洗进气道(4S)  402001:添加新车保护剂(4S)  402001:发动机润滑系统清洗(4S)  402001:发动机内部保护(4S)
        /// </summary>
        [XmlElement("service_category_id")]
        public long ServiceCategoryId { get; set; }

        /// <summary>
        /// 服务商品状态（0：下线；1：上线）。下线后门店详情页内不透出。  新增：必填
        /// </summary>
        [XmlElement("status")]
        public string Status { get; set; }
    }
}

using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEcoLogisticsExpressOrderModifyModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEcoLogisticsExpressOrderModifyModel : AopObject
    {
        /// <summary>
        /// 接单类型，已接单状态时必填  courier_accept：快递员接单，  site_accept：快递站接单
        /// </summary>
        [XmlElement("accept_type")]
        public string AcceptType { get; set; }

        /// <summary>
        /// 快递员支付宝帐号，快递员接单时选填
        /// </summary>
        [XmlElement("courier_alipay_account")]
        public string CourierAlipayAccount { get; set; }

        /// <summary>
        /// 员工号，快递员接单时必填
        /// </summary>
        [XmlElement("courier_emp_num")]
        public string CourierEmpNum { get; set; }

        /// <summary>
        /// 接单快递员头像文件二进制内容的BASE64编码串  头像格式为100X100的PNG图片
        /// </summary>
        [XmlElement("courier_head_img")]
        public string CourierHeadImg { get; set; }

        /// <summary>
        /// 身份证，快递员接单时选填
        /// </summary>
        [XmlElement("courier_id_card")]
        public string CourierIdCard { get; set; }

        /// <summary>
        /// 快递员手机号，快递员接单时必填
        /// </summary>
        [XmlElement("courier_mobile")]
        public string CourierMobile { get; set; }

        /// <summary>
        /// 快递员姓名，快递员接单时必填
        /// </summary>
        [XmlElement("courier_name")]
        public string CourierName { get; set; }

        /// <summary>
        /// 物品重量（克），已取件状态时必填
        /// </summary>
        [XmlElement("goods_weight")]
        public long GoodsWeight { get; set; }

        /// <summary>
        /// 物流机构编码，参照物流机构编码文档，<a href="https://expressprod.oss-cn-hangzhou.aliyuncs.com/docs/%E5%AF%84%E4%BB%B6%E5%B9%B3%E5%8F%B0-%E7%89%A9%E6%B5%81%E6%9C%BA%E6%9E%84%E7%BC%96%E7%A0%81%E6%96%87%E6%A1%A3.xlsx">点此下载</a>。
        /// </summary>
        [XmlElement("logis_merch_code")]
        public string LogisMerchCode { get; set; }

        /// <summary>
        /// 订单金额（元），已取件状态时必填
        /// </summary>
        [XmlElement("order_amount")]
        public string OrderAmount { get; set; }

        /// <summary>
        /// 寄件平台订单号，系统唯一
        /// </summary>
        [XmlElement("order_no")]
        public string OrderNo { get; set; }

        /// <summary>
        /// 订单状态  collected:已取件  accepted:已接单  canceled:已取消
        /// </summary>
        [XmlElement("order_status")]
        public string OrderStatus { get; set; }

        /// <summary>
        /// 产品类型变更原因，产品类型变更时选填。
        /// </summary>
        [XmlElement("product_type_change_reason")]
        public string ProductTypeChangeReason { get; set; }

        /// <summary>
        /// 产品类型编码，如果快递公司变更了产品类型，则需要将变更后的产品类型编码回传。取值如下：  STANDARD：标准快递。这是寄件平台默认支持的产品分类，如有其他产品分类的支持需求，可以发送邮件至xxx@alipay.com联系。
        /// </summary>
        [XmlElement("product_type_code")]
        public string ProductTypeCode { get; set; }

        /// <summary>
        /// 快递公司拒绝接单原因编码，已取消状态时必填，取值如下：  R01  揽收地超服务范围  R02  派送地超服务范围  R03  揽收预约时间超范围，无法协商  R04  虚假揽货电话（客户电话与联系人不符）  R05  托寄物品为禁限寄品  R06  用户恶意下单  R07  用户取消订单  R08  其他原因，需自定义描述
        /// </summary>
        [XmlElement("refuse_code")]
        public string RefuseCode { get; set; }

        /// <summary>
        /// 快递公司拒绝接单原因描述，快递公司拒绝接单原因编码取值为R08时必填
        /// </summary>
        [XmlElement("refuse_desc")]
        public string RefuseDesc { get; set; }

        /// <summary>
        /// 站点所在区县编码，快递站点接单时必填。采用国家标准编码，详见国家统计局数据，<a href="http://aopsdkdownload.cn-hangzhou.alipay-pub.aliyun-inc.com/doc/2016.xls?spm=a219a.7395905.0.0.IwSSLe&file=2016.xls">点此下载</a>。
        /// </summary>
        [XmlElement("site_area_code")]
        public string SiteAreaCode { get; set; }

        /// <summary>
        /// 快递站点编号，快递站点接单时必填
        /// </summary>
        [XmlElement("site_code")]
        public string SiteCode { get; set; }

        /// <summary>
        /// 站点投诉电话，快递站点接单时选填
        /// </summary>
        [XmlElement("site_complain_tel")]
        public string SiteComplainTel { get; set; }

        /// <summary>
        /// 站点所在详细地址，快递站点接单时必填
        /// </summary>
        [XmlElement("site_detail_addr")]
        public string SiteDetailAddr { get; set; }

        /// <summary>
        /// 站点负责人手机号，快递站点接单时必填
        /// </summary>
        [XmlElement("site_leader_mobile")]
        public string SiteLeaderMobile { get; set; }

        /// <summary>
        /// 站点负责人，快递站点接单时必填
        /// </summary>
        [XmlElement("site_leader_name")]
        public string SiteLeaderName { get; set; }

        /// <summary>
        /// 快递站点名称，快递站点接单时必填
        /// </summary>
        [XmlElement("site_name")]
        public string SiteName { get; set; }

        /// <summary>
        /// 运单号，已取件状态时必填
        /// </summary>
        [XmlElement("way_bill_no")]
        public string WayBillNo { get; set; }
    }
}

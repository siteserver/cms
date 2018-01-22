using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEcoLogisticsExpressPriceQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEcoLogisticsExpressPriceQueryModel : AopObject
    {
        /// <summary>
        /// 查询区域类型  AREA_PRVN:省代码；  AREA_CITY:市代码；
        /// </summary>
        [XmlElement("area_type")]
        public string AreaType { get; set; }

        /// <summary>
        /// 发货区域代码  区域类型为省代码时为省代码；  区域类型为市代码时为市代码；  省市区代码采用国家标准编码，详见国家统计局数据，<a href="http://aopsdkdownload.cn-hangzhou.alipay-pub.aliyun-inc.com/doc/2016.xls?spm=a219a.7395905.0.0.IwSSLe&file=2016.xls">点此下载</a>。
        /// </summary>
        [XmlElement("from_code")]
        public string FromCode { get; set; }

        /// <summary>
        /// 物流机构编码，参照物流机构编码文档，<a href="https://expressprod.oss-cn-hangzhou.aliyuncs.com/docs/%E5%AF%84%E4%BB%B6%E5%B9%B3%E5%8F%B0-%E7%89%A9%E6%B5%81%E6%9C%BA%E6%9E%84%E7%BC%96%E7%A0%81%E6%96%87%E6%A1%A3.xlsx">点此下载</a>。
        /// </summary>
        [XmlElement("logis_merch_code")]
        public string LogisMerchCode { get; set; }

        /// <summary>
        /// 产品类型编码，取值如下：  STANDARD：标准快递。这是寄件平台默认支持的产品分类，如有其他产品分类的支持需求，可以发送邮件至xxx@alipay.com联系。
        /// </summary>
        [XmlElement("product_type_code")]
        public string ProductTypeCode { get; set; }

        /// <summary>
        /// 收货区域代码  区域类型为省代码时为省代码；  区域类型为市代码时为市代码；  省市区代码采用国家标准编码，详见国家统计局数据，<a href="http://aopsdkdownload.cn-hangzhou.alipay-pub.aliyun-inc.com/doc/2016.xls?spm=a219a.7395905.0.0.IwSSLe&file=2016.xls">点此下载</a>。
        /// </summary>
        [XmlElement("to_code")]
        public string ToCode { get; set; }
    }
}

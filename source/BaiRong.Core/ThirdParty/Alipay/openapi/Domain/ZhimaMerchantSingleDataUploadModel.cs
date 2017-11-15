using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// ZhimaMerchantSingleDataUploadModel Data Structure.
    /// </summary>
    [Serializable]
    public class ZhimaMerchantSingleDataUploadModel : AopObject
    {
        /// <summary>
        /// 公用回传参数（非必填），这个字段由商户传入，系统透传给商户，便于商户做逻辑关联，请使用json格式。
        /// </summary>
        [XmlElement("biz_ext_params")]
        public string BizExtParams { get; set; }

        /// <summary>
        /// 传入的json数据，商户通过json格式将数据传给芝麻 ， json中的字段可以通过如下步骤获取：首先调用zhima.merchant.data.upload.initialize接口获取数据模板，该接口会返回一个数据模板文件的url地址，如：http://zmxymerchant-prod.oss-cn-shenzhen.zmxy.com.cn/openApi/openDoc/信用护航-负面记录和信用足迹商户数据模板V1.0.xlsx，该数据模板文件详细列出了需要传入的字段，及各字段的要求，data中的各字段就是该文件中列出的字段编码。
        /// </summary>
        [XmlElement("data")]
        public string Data { get; set; }

        /// <summary>
        /// 主键列使用传入字段进行组合，也可以使用传入的某个单字段（确保主键稳定，而且可以很好的区分不同的数据）。例如order_no,pay_month或者order_no,bill_month组合，对于一个order_no只会有一条数据的情况，直接使用order_no作为主键列 。
        /// </summary>
        [XmlElement("primary_keys")]
        public string PrimaryKeys { get; set; }

        /// <summary>
        /// 数据应用的场景编码 ，场景码和场景名称（数字为场景码）如下：  1:负面披露  2:信用足迹  3:负面+足迹  4:信用守护  5:负面+守护  6:足迹+守护  7:负面+足迹+守护  8:数据反馈  32:骑行  每个场景码对应的数据模板不一样，请使用zhima.merchant.data.upload.initialize接口获取场景码对应的数据模板。
        /// </summary>
        [XmlElement("scene_code")]
        public string SceneCode { get; set; }
    }
}

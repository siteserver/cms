using System;
using System.Xml.Serialization;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayEcoMedicalcareCommonDataSyncModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayEcoMedicalcareCommonDataSyncModel : AopObject
    {
        /// <summary>
        /// 为了区分相同ISV不同应用的编码，数据来源是类目平台  当data_type为CALLBACK时必填
        /// </summary>
        [XmlElement("app_code")]
        public string AppCode { get; set; }

        /// <summary>
        /// 业务主体，data_body根据不同的模板ID传入对应的JSON格式  注意：业务data_body与模板teaplate_id对应，具体实例：  http://medicalcare.oss-cn-hangzhou.aliyuncs.com/prod/data/transfer/[template_id].html
        /// </summary>
        [XmlElement("data_body")]
        public string DataBody { get; set; }

        /// <summary>
        /// 业务数据类型  APP 应用类Card  REMIND 提醒类Card  CALLBACK  数据回流
        /// </summary>
        [XmlElement("data_type")]
        public string DataType { get; set; }

        /// <summary>
        /// 医疗服务平台提供数据模板ID
        /// </summary>
        [XmlElement("template_id")]
        public string TemplateId { get; set; }

        /// <summary>
        /// 支付宝用户ID,可以通过获取会员信息产品 获取支付宝用户ID  当data_type为CALLBACK时必填
        /// </summary>
        [XmlElement("user_id")]
        public string UserId { get; set; }
    }
}

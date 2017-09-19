using System;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Aop.Api.Domain
{
    /// <summary>
    /// AlipayMarketingDataModelQueryModel Data Structure.
    /// </summary>
    [Serializable]
    public class AlipayMarketingDataModelQueryModel : AopObject
    {
        /// <summary>
        /// 模型查询输入参数格式。此为参数列表，参数包含外部用户身分信息类型、模型输出字段及模型输出值，根据实际业务需求获取；用于实验试算法模型结果查询  key:条件查询参数。此为外部用户身份信息类型，例如:手机号、身份证  operate:操作计算符数。此为查询条件  value:查询参数值。此为查询值
        /// </summary>
        [XmlArray("model_query_param")]
        [XmlArrayItem("model_query_param")]
        public List<ModelQueryParam> ModelQueryParam { get; set; }

        /// <summary>
        /// 模型唯一查询标识符。参数值为调用batchquery接口后获取的model_uk参数值；用于标识模型的唯一性
        /// </summary>
        [XmlElement("model_uk")]
        public string ModelUk { get; set; }
    }
}

using System;
using System.Xml.Serialization;

namespace Aop.Api.Response
{
    /// <summary>
    /// AlipayCommerceDataMonitordataSyncResponse.
    /// </summary>
    public class AlipayCommerceDataMonitordataSyncResponse : AopResponse
    {
        /// <summary>
        /// dimTypeCheck: 维度类型校验    [error]： 错误原因，由错误码，错误描述组成    [operation]：处理措施，如忽略维度，抛弃维度信息    [indicator]: 哪个指标发生的错误  ［source]：错误的地方，如上面写的para，维度类型写错    如果有多个维度校验类型错误，多个错误将组合在一起返回    {dataTypeValueCheck=    [error]:Data_Type_Value_Invalid,数据或类型不合法    [operation]:忽略数据         [indicator]:当面付成功交易数    [source]:type:String value:10    [error]:Data_Type_Value_Invalid,数据或类型不合法    [operation]:忽略数据    [indicator]:当面付成功交易数    [source]:type:String value:20    [error]:Data_Type_Value_Invalid,数据或类型不合法    [operation]:忽略数据    [indicator]:当面付成功交易数    [source]:type:String value:30}
        /// </summary>
        [XmlElement("error_detail")]
        public string ErrorDetail { get; set; }
    }
}

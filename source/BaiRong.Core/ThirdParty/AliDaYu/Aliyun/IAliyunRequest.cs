using System.Collections.Generic;

namespace Aliyun.Api
{
    /// <summary>
    /// TOP请求接口。
    /// </summary>
    public interface IAliyunRequest<T> where T : AliyunResponse
    {
        /// <summary>
        /// 获取TOP的API名称。
        /// </summary>
        /// <returns>API名称</returns>
        string GetApiName();

        /// <summary>
        /// 获取所有的Key-Value形式的文本请求参数字典。其中：
        /// Key: 请求参数名
        /// Value: 请求参数文本值
        /// </summary>
        /// <returns>文本请求参数字典</returns>
        IDictionary<string, string> GetParameters();

        /// <summary>
        /// 提前验证参数。
        /// </summary>
        void Validate();
    }
}

using System.Collections.Generic;

namespace DingTalk.Api
{
    /// <summary>
    /// TOP请求接口。
    /// </summary>
    public interface IDingTalkRequest<out T> where T : DingTalkResponse
    {
        /// <summary>
        /// 获取TOP的API名称。
        /// </summary>
        string GetApiName();

        /// <summary>
        /// 获取所有的Key-Value形式的文本请求参数字典。
        /// </summary>
        IDictionary<string, string> GetParameters();

        /// <summary>
        /// 获取自定义HTTP请求头参数。
        /// </summary>
        IDictionary<string, string> GetHeaderParameters();

        /// <summary>
        /// 客户端参数检查，减少服务端无效调用。
        /// </summary>
        void Validate();
    }
}

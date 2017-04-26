using System.Collections.Generic;

namespace Top.Api
{
    /// <summary>
    /// TOP请求接口。
    /// </summary>
    public interface ITopRequest<out T> where T : TopResponse
    {
        /// <summary>
        /// 获取TOP的API名称。
        /// </summary>
        string GetApiName();

        /// <summary>
        /// 获取被调用的目标AppKey
        /// </summary>
        string GetTargetAppKey();

        /// <summary>
        /// 获取所有的Key-Value形式的文本请求参数字典。
        /// </summary>
        IDictionary<string, string> GetParameters();

        /// <summary>
        /// 获取自定义HTTP请求头参数。
        /// </summary>
        IDictionary<string, string> GetHeaderParameters();

        /// <summary>
        ///  获取API请求的用户授权码，仅用于批量API调用请求。
        /// </summary>
        string GetBatchApiSession();

        /// <summary>
        /// 设置API请求的用户授权码，仅用于批量API调用请求。
        /// </summary>
        void SetBatchApiSession(string session);

        /// <summary>
        /// 获取API在批量调用中的顺序，仅用于批量API调用请求。
        /// </summary>
        int GetBatchApiOrder();

        /// <summary>
        /// 设置API在批量调用中的顺序，仅用于批量API调用请求。
        /// </summary>
        void SetBatchApiOrder(int order);

        /// <summary>
        /// 客户端参数检查，减少服务端无效调用。
        /// </summary>
        void Validate();
    }
}

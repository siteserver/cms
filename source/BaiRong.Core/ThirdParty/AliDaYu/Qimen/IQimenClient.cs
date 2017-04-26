namespace Qimen.Api
{
    /// <summary>
    /// 奇门客户端。
    /// </summary>
    public interface IQimenClient
    {
        /// <summary>
        /// 执行奇门公开API请求。
        /// </summary>
        /// <typeparam name="T">领域对象</typeparam>
        /// <param name="request">具体的奇门API请求</param>
        /// <returns>领域对象</returns>
        T Execute<T>(QimenRequest<T> request) where T : QimenResponse;

        /// <summary>
        /// 执行奇门隐私API请求。
        /// </summary>
        /// <typeparam name="T">领域对象</typeparam>
        /// <param name="request">具体的奇门API请求</param>
        /// <param name="session">用户会话码</param>
        /// <returns>领域对象</returns>
        T Execute<T>(QimenRequest<T> request, string session) where T : QimenResponse;
    }
}

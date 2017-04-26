using System;

namespace DingTalk.Api
{
    /// <summary>
    /// TOP客户端。
    /// </summary>
    public interface IDingTalkClient
    {
        /// <summary>
        /// 执行TOP公开API请求。
        /// </summary>
        /// <typeparam name="T">领域对象</typeparam>
        /// <param name="request">具体的TOP API请求</param>
        /// <returns>领域对象</returns>
        T Execute<T>(IDingTalkRequest<T> request) where T : DingTalkResponse;

        /// <summary>
        /// 执行TOP隐私API请求。
        /// </summary>
        /// <typeparam name="T">领域对象</typeparam>
        /// <param name="request">具体的TOP API请求</param>
        /// <param name="session">用户会话码</param>
        /// <returns>领域对象</returns>
        T Execute<T>(IDingTalkRequest<T> request, string session) where T : DingTalkResponse;

        /// <summary>
        /// 执行TOP隐私API请求。
        /// </summary>
        /// <typeparam name="T">领域对象</typeparam>
        /// <param name="request">具体的TOP API请求</param>
        /// <param name="session">用户会话码</param>
        /// <param name="timestamp">请求时间戳</param>
        /// <returns>领域对象</returns>
        T Execute<T>(IDingTalkRequest<T> request, string session, DateTime timestamp) where T : DingTalkResponse;
    }
}

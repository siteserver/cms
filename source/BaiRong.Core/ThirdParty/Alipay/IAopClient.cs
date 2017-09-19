using Aop.Api.Request;

namespace Aop.Api
{
    /// <summary>
    /// AOP客户端。
    /// </summary>
    public interface IAopClient
    {
        /// <summary>
        /// 执行AOP公开API请求。
        /// </summary>
        /// <typeparam name="T">领域对象</typeparam>
        /// <param name="request">具体的AOP API请求</param>
        /// <returns>领域对象</returns>
        T Execute<T>(IAopRequest<T> request) where T : AopResponse;

        /// <summary>
        /// 执行AOP隐私API请求。
        /// </summary>
        /// <typeparam name="T">领域对象</typeparam>
        /// <param name="request">具体的AOP API请求</param>
        /// <param name="session">用户会话码</param>
        /// <returns>领域对象</returns>
        T Execute<T>(IAopRequest<T> request, string session) where T : AopResponse;


           /// <summary>
        /// 执行AOP隐私API请求。
        /// </summary>
        /// <typeparam name="T">领域对象</typeparam>
        /// <param name="request">具体的AOP API请求</param>
        /// <param name="session">用户会话码</param>
        /// <param name="appAuthToken">应用授权码</param>
        /// <returns>领域对象</returns>
        T Execute<T>(IAopRequest<T> request, string session,string appAuthToken) where T : AopResponse;

        //add 
        /// <summary>
        /// 执行AOP公开API请求。
        /// </summary>
        /// <typeparam name="T">领域对象</typeparam>
        /// <param name="request">具体的AOP API请求</param>
        /// <returns>领域对象</returns>
        T pageExecute<T>(IAopRequest<T> request) where T : AopResponse;

        /// <summary>
        /// 执行AOP隐私API请求。
        /// </summary>
        /// <typeparam name="T">领域对象</typeparam>
        /// <param name="request">具体的AOP API请求</param>
        /// <param name="session">用户会话码</param>
        /// <returns>领域对象</returns>
        T pageExecute<T>(IAopRequest<T> request, string session, string reqMethod) where T : AopResponse;

        /// <summary>
        /// 执行AOP公开API请求。
        /// </summary>
        /// <typeparam name="T">领域对象</typeparam>
        /// <param name="request">具体的AOP API请求</param>
        /// <returns>领域对象</returns>
        T SdkExecute<T>(IAopRequest<T> request) where T : AopResponse;
    }
}

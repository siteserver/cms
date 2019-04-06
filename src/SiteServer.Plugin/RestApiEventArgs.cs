using System;

namespace SiteServer.Plugin
{
    /// <summary>
    /// Rest Api
    /// </summary>
    /// <remarks>
    /// 为Rest Api请求事件提供数据。
    /// </remarks>
    public class RestApiEventArgs : EventArgs
    {
        /// <summary>
        /// 初始化 <see cref="T:SiteServer.Plugin.RestApiEventArgs" /> 类的新实例。
        /// </summary>
        /// <param name="request">Rest Api 请求对象实例。</param>
        /// <param name="route">Rest Api 请求路径</param>
        /// <param name="routeResource">Rest Api 请求路径中包含的资源名称。</param>
        /// <param name="routeId">Rest Api 请求路径中包含的资源Id。</param>
        /// <param name="routeAction">Rest Api 请求路径中包含的动作名称。</param>
        public RestApiEventArgs(IRequest request, string route, string routeResource, string routeId, string routeAction)
        {
            Request = request;
            Route = route;
            RouteResource = routeResource;
            RouteId = routeId;
            RouteAction = routeAction;
        }

        /// <summary>
        /// Rest Api 请求对象实例 <see cref="T:SiteServer.Plugin.IRequest" />。
        /// </summary>
        public IRequest Request { get; }

        /// <summary>
        /// Rest Api 请求路径。
        /// </summary>
        public string Route { get; }

        /// <summary>
        /// Rest Api 请求路径中包含的资源名称。
        /// </summary>
        public string RouteResource { get; }

        /// <summary>
        /// Rest Api 请求路径中包含的资源Id。
        /// </summary>
        public string RouteId { get; }

        /// <summary>
        /// Rest Api 请求路径中包含的动作名称。
        /// </summary>
        public string RouteAction { get; }
    }
}
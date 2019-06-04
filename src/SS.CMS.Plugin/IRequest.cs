using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Http;

namespace SS.CMS.Plugin
{
    /// <summary>
    /// 表示与请求关联的上下文。
    /// </summary>
    public partial interface IRequest
    {
        /// <summary>
        /// Gets an object that can be used to manage cookies for this response.
        /// </summary>
        IRequestCookieCollection Cookies { get; }

        ///// <summary>Gets the request headers.</summary>
        IHeaderDictionary Headers { get; }

        IDictionary<string, object> RouteValues { get; }

        /// <summary>
        /// Returns true if the RequestScheme is https.
        /// </summary>
        /// <returns>true if this request is using https; otherwise, false.</returns>
        bool IsHttps { get; }

        /// <summary>
        /// Gets or sets the Host header. May include the port.
        /// </summary>
        /// <return>The Host header.</return>
        string Host { get; }

        /// <summary>
        /// Gets or sets the request path from RequestPath.
        /// </summary>
        /// <returns>The request path from RequestPath.</returns>
        string Path { get; }

        string RawUrl { get; }

        string IpAddress { get; }

        Stream Body { get; }

        string AdminToken { get; }

        string UserToken { get; }

        string ApiToken { get; }
    }
}
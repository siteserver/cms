using Microsoft.AspNetCore.Http;

namespace SS.CMS.Plugin
{
    public partial interface IResponse
    {
        /// <summary>
        /// Gets an object that can be used to manage cookies for this response.
        /// </summary>
        IResponseCookies Cookies { get; }

        /// <summary>Gets the response headers.</summary>
        IHeaderDictionary Headers { get; }
    }
}

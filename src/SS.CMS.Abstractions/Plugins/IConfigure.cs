using System;
using Microsoft.AspNetCore.Builder;

namespace SS.CMS.Abstractions.Plugins
{
    /// <summary>
    /// Describes an action that must be executed inside the Configure method of a web application's Startup class
    /// and might be used by the extensions to configure a web application's request pipeline.
    /// </summary>
    public interface IConfigure
    {
        /// <summary>
        /// Contains any code that must be executed inside the Configure method of the web application's Startup class.
        /// </summary>
        void Configure(IApplicationBuilder app, IServiceProvider provider);
    }
}
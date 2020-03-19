using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SSCMS.Plugins;

namespace SSCMS.Core.Plugins
{
    /// <summary>
    /// Implements the <see cref="IConfigure">IConfigureAction</see> interface and registers the
    /// static files middleware inside a web application's request pipeline.
    /// </summary>
    public class UseStaticFiles : IConfigure
    {
        /// <summary>
        /// Registers the static files middleware inside a web application's request pipeline.
        /// </summary>
        public void Configure(IApplicationBuilder app, IServiceProvider provider)
        {
            var options = provider.GetService<IOptions<StaticFileOptions>>();
            app.UseStaticFiles(options?.Value);
        }
    }
}
using System;
using Microsoft.Extensions.DependencyInjection;

namespace SSCMS.Plugins
{
    /// <summary>
    /// Describes an action that must be executed inside the ConfigureServices method of a web application's Startup class
    /// and might be used by the extensions to register any service inside the DI.
    /// </summary>
    public interface IConfigureServices
    {
        /// <summary>
        /// Contains any code that must be executed inside the ConfigureServices method of the web application's Startup class.
        /// </summary>
        void ConfigureServices(IServiceCollection services, IServiceProvider provider);
    }
}
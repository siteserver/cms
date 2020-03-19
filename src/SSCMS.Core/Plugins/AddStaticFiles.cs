using System;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using SSCMS.Plugins;

namespace SSCMS.Core.Plugins
{
    /// <summary>
    /// Implements the <see cref="IConfigureServices">IConfigureServicesAction</see> interface and
    /// creates and registers the composite file provider that contains resources from all the extensions.
    /// </summary>
    public class AddStaticFiles : IConfigureServices
    {
        /// <summary>
        /// Creates and registers the composite file provider that contains resources from all the extensions.
        /// </summary>
        public void ConfigureServices(IServiceCollection services, IServiceProvider provider)
        {
            provider.GetService<IWebHostEnvironment>().WebRootFileProvider =
                CreateCompositeFileProvider(provider);
        }

        private static IFileProvider CreateCompositeFileProvider(IServiceProvider serviceProvider)
        {
            var fileProviders = new[]
            {
                serviceProvider.GetService<IWebHostEnvironment>().WebRootFileProvider
            };

            return new CompositeFileProvider(
                fileProviders.Concat(
                    AssemblyUtils.Assemblies.Select(a => new EmbeddedFileProvider(a, a.GetName().Name))
                )
            );
        }
    }
}
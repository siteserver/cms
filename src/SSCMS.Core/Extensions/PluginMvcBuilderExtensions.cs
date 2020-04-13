using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using SSCMS.Plugins;
using SSCMS.Services;

namespace SSCMS.Core.Extensions
{
    public static class PluginMvcBuilderExtensions
    {
        public static IMvcBuilder AddPluginApplicationParts(
            this IMvcBuilder mvcBuilder,
            IPluginManager pluginManager)
        {
            foreach (var plugin in pluginManager.Plugins)
            {
                AddApplicationPart(mvcBuilder.PartManager, plugin);
            }
            return mvcBuilder;
        }

        private static void AddApplicationPart(ApplicationPartManager applicationPartManager, IPluginMetadata pluginMetadata)
        {
            var partFactory = ApplicationPartFactory.GetApplicationPartFactory(pluginMetadata.Assembly);

            foreach (var part in partFactory.GetApplicationParts(pluginMetadata.Assembly))
            {
                applicationPartManager.ApplicationParts.Add(part);
            }

            var relatedAssemblies = RelatedAssemblyAttribute.GetRelatedAssemblies(pluginMetadata.Assembly, throwOnError: true);
            foreach (var assembly in relatedAssemblies)
            {
                partFactory = ApplicationPartFactory.GetApplicationPartFactory(assembly);
                foreach (var part in partFactory.GetApplicationParts(assembly))
                {
                    applicationPartManager.ApplicationParts.Add(part);
                }
            }
        }
    }
}

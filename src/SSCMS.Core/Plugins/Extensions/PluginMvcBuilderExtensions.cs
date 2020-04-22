using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
using SSCMS.Plugins;
using SSCMS.Services;

namespace SSCMS.Core.Plugins.Extensions
{
    public static class PluginMvcBuilderExtensions
    {
        public static IMvcBuilder AddPluginApplicationParts(
            this IMvcBuilder mvcBuilder,
            IPluginManager pluginManager)
        {
            foreach (var plugin in pluginManager.Plugins)
            {
                if (plugin.Assembly != null)
                {
                    AddApplicationPart(mvcBuilder.PartManager, plugin);
                }
            }
            return mvcBuilder;
        }

        private static void AddApplicationPart(ApplicationPartManager applicationPartManager, IPlugin plugin)
        {
            var partFactory = ApplicationPartFactory.GetApplicationPartFactory(plugin.Assembly);

            foreach (var part in partFactory.GetApplicationParts(plugin.Assembly))
            {
                applicationPartManager.ApplicationParts.Add(part);
            }

            var relatedAssemblies = RelatedAssemblyAttribute.GetRelatedAssemblies(plugin.Assembly, throwOnError: true);
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

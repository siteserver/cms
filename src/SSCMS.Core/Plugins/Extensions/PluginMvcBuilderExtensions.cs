using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.DependencyInjection;
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
                    AddApplicationPart(mvcBuilder.PartManager, plugin.Assembly);
                }
            }
            return mvcBuilder;
        }

        private static void AddApplicationPart(ApplicationPartManager applicationPartManager, Assembly assembly)
        {
            var partFactory = ApplicationPartFactory.GetApplicationPartFactory(assembly);

            foreach (var part in partFactory.GetApplicationParts(assembly))
            {
                applicationPartManager.ApplicationParts.Add(part);
            }

            var relatedAssemblies = RelatedAssemblyAttribute.GetRelatedAssemblies(assembly, throwOnError: true);
            foreach (var relatedAssembly in relatedAssemblies)
            {
                partFactory = ApplicationPartFactory.GetApplicationPartFactory(relatedAssembly);
                foreach (var part in partFactory.GetApplicationParts(relatedAssembly))
                {
                    applicationPartManager.ApplicationParts.Add(part);
                }
            }
        }
    }
}

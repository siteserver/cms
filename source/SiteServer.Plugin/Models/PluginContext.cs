namespace SiteServer.Plugin.Models
{
    public class PluginContext
    {
        public PluginContext(PluginEnvironment environment, PluginMetadata metadata, IPublicApi api)
        {
            Environment = environment;
            Metadata = metadata;
            Api = api;
        }

        public PluginEnvironment Environment { get; }

        public PluginMetadata Metadata { get; set; }

        /// <summary>
        /// 公开的API调用
        /// </summary>
        public IPublicApi Api { get; }
    }
}

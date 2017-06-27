namespace SiteServer.Plugin
{
    public class PluginPair
    {
        public PluginPair(PluginContext context, IPlugin plugin)
        {
            Context = context;
            Plugin = plugin;
        }

        public PluginContext Context { get; }

        public PluginMetadata Metadata => Context.Metadata;

        public IPlugin Plugin { get; }
    }
}

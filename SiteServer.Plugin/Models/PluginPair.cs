namespace SiteServer.Plugin.Models
{
    public class PluginPair
    {
        public PluginPair(PluginContext context, IPlugin plugin)
        {
            Context = context;
            Plugin = plugin;
        }

        public PluginContext Context { get; }

        public PluginMetadata Metadata
        {
            get { return Context.Metadata; }
            set { Context.Metadata = value; }
        }

        public IPlugin Plugin { get; }
    }
}

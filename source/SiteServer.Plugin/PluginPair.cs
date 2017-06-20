namespace SiteServer.Plugin
{
    public class PluginPair
    {
        public PluginPair(PluginMetadata metadata, IPlugin plugin)
        {
            Metadata = metadata;
            Plugin = plugin;
        }

        public IPlugin Plugin { get; }
        public PluginMetadata Metadata { get; }

        public override string ToString()
        {
            return Metadata.Name;
        }

        public override bool Equals(object obj)
        {
            var r = obj as PluginPair;
            return r != null && string.Equals(r.Metadata.Id, Metadata.Id);
        }

        public override int GetHashCode()
        {
            var hashcode = Metadata.Id?.GetHashCode() ?? 0;
            return hashcode;
        }
    }
}

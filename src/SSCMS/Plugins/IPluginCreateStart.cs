using SSCMS.Parse;

namespace SSCMS.Plugins
{
    public interface IPluginCreateStart : IPluginExtension
    {
        void Parse(IParseContext context);
    }
}

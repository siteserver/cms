using SSCMS.Context;

namespace SSCMS.Plugins
{
    public interface IPluginCreateStart : IPluginExtension
    {
        void Parse(IParseContext context);
    }
}

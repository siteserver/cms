using SSCMS.Context;

namespace SSCMS.Plugins
{
    public interface IPluginCreateEnd : IPluginExtension
    {
        void Parse(IParseContext context);
    }
}

using SSCMS.Parse;

namespace SSCMS.Plugins
{
    public interface IPluginCreateEnd : IPluginExtension
    {
        void Parse(IParseContext context);
    }
}

using SSCMS.Context;

namespace SSCMS.Plugins
{
    public interface IPluginParse : IPluginExtension
    {
        string ElementName { get; }
        string Parse(IParseStlContext context);
    }
}

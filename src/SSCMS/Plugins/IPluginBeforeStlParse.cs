namespace SSCMS.Plugins
{
    public interface IPluginBeforeStlParse : IPluginExtension
    {
        void BeforeStlParse(IStlParseContext context);
    }
}

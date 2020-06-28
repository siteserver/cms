namespace SSCMS.Plugins
{
    public interface IPluginAfterStlParse : IPluginExtension
    {
        void AfterStlParse(IStlParseContext context);
    }
}

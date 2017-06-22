namespace SiteServer.Plugin
{
    public interface IPlugin
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="context"></param>
        void Initialize(PluginContext context);

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务
        /// </summary>
        /// <param name="context"></param>
        void Dispose(PluginContext context);
    }
}
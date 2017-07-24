namespace SiteServer.Plugin
{
    public interface IPlugin
    {
        /// <summary>
        /// 激活插件，执行初始化
        /// </summary>
        /// <param name="context"></param>
        void Active(PluginContext context);

        /// <summary>
        /// 停止插件，执行与释放或重置非托管资源相关的应用程序定义的任务
        /// </summary>
        /// <param name="context"></param>
        void Deactive(PluginContext context);

        /// <summary>
        /// 卸载插件
        /// </summary>
        /// <param name="context"></param>
        void Uninstall(PluginContext context);
    }
}

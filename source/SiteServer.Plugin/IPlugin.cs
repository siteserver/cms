using SiteServer.Plugin.Models;
using System;

namespace SiteServer.Plugin
{
    public interface IPlugin
    {
        /// <summary>
        /// 激活插件，执行初始化
        /// </summary>
        Action<PluginContext> OnPluginActive { get; }

        /// <summary>
        /// 停止插件，执行与释放或重置非托管资源相关的应用程序定义的任务
        /// </summary>
        Action<PluginContext> OnPluginDeactive { get; }

        /// <summary>
        /// 卸载插件
        /// </summary>
        Action<PluginContext> OnPluginUninstall { get; }
    }
}

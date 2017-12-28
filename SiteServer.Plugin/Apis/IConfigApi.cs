using SiteServer.Plugin.Models;

namespace SiteServer.Plugin.Apis
{
    public interface IConfigApi
    {
        bool SetConfig(int publishmentSystemId, string name, object config);

        bool SetConfig(int publishmentSystemId, object config);

        T GetConfig<T>(int publishmentSystemId, string name = "");

        bool RemoveConfig(int publishmentSystemId, string name = "");

        ISystemConfigInfo SystemConfigInfo { get; }
    }
}

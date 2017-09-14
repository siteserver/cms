namespace SiteServer.Plugin.Apis
{
    public interface IConfigApi
    {
        bool SetGlobalConfig(object config);

        bool SetGlobalConfig(string name, object config);

        T GetGlobalConfig<T>(string name = "");

        bool RemoveGlobalConfig(string name = "");

        bool SetConfig(int publishmentSystemId, string name, object config);

        bool SetConfig(int publishmentSystemId, object config);

        T GetConfig<T>(int publishmentSystemId, string name = "");

        bool RemoveConfig(int publishmentSystemId, string name = "");

        string EncryptStringBySecretKey(string inputString);

        string DecryptStringBySecretKey(string inputString);

        string PhysicalApplicationPath { get; }

        string AdminDirectory { get; }
    }
}

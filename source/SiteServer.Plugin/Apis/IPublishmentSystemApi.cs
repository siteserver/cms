using System.Collections.Generic;
using SiteServer.Plugin.Models;

namespace SiteServer.Plugin.Apis
{
    public interface IPublishmentSystemApi
    {
        int GetPublishmentSystemIdByFilePath(string path);

        string GetPublishmentSystemPath(int publishmentSystemId);

        List<int> GetPublishmentSystemIds();

        IPublishmentSystemInfo GetPublishmentSystemInfo(int publishmentSystemId);
    }
}

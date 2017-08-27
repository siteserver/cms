using System;
using System.Collections.Generic;
using SiteServer.Plugin.Models;

namespace SiteServer.Plugin.Apis
{
    public interface IPluginApi
    {
        void AddErrorLog(Exception ex);

        string GetPluginPageUrl(int publishmentSystemId, string relatedUrl = "");

        string GetPluginJsonApiUrl(int publishmentSystemId, string action = "", int id = 0);

        string GetPluginHttpApiUrl(int publishmentSystemId, string action = "", int id = 0);
    }
}

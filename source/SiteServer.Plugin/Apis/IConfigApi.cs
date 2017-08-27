using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SiteServer.Plugin.Models;

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

        string PhysicalApplicationPath { get; }

        string AdminDirectory { get; }
    }
}

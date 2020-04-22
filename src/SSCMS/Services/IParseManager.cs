using System;
using System.Text;
using System.Threading.Tasks;
using SSCMS.Models;
using SSCMS.Parse;

namespace SSCMS.Services
{
    public partial interface IParseManager
    {
        ISettingsManager SettingsManager { get; }
        IPathManager PathManager { get; }
        IDatabaseManager DatabaseManager { get; }
        IOldPluginManager OldPluginManager { get; }
        ParsePage PageInfo { get; set; }
        ParseContext ContextInfo { get; set; }

        Task InitAsync(Site site, int pageChannelId, int pageContentId, Template template);

        Task ParseAsync(StringBuilder contentBuilder, string filePath, bool isDynamic);

        Task<string> AddStlErrorLogAsync(string elementName, string stlContent, Exception ex);

        Task<Channel> GetChannelAsync();

        Task<Content> GetContentAsync();
    }
}

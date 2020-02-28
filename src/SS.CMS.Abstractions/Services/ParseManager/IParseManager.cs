
using System;
using System.Text;
using System.Threading.Tasks;
using SS.CMS.Abstractions.Parse;
using StackExchange.Redis;

namespace SS.CMS.Abstractions
{
    public partial interface IParseManager : IService
    {
        ISettingsManager SettingsManager { get; }
        IPathManager PathManager { get; }
        IDatabaseManager DatabaseManager { get; }
        ParsePage PageInfo { get; }
        ParseContext ContextInfo { get; set; }

        Task ParseAsync(ParsePage pageInfo, ParseContext contextInfo, StringBuilder contentBuilder,
            string filePath, bool isDynamic);

        Task<string> AddStlErrorLogAsync(ParsePage pageInfo, string elementName, string stlContent, Exception ex);

        Task<Channel> GetChannelAsync();

        Task<Content> GetContentAsync();
    }
}

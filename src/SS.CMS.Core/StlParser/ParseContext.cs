using System.Collections.Specialized;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SS.CMS.Core.StlParser.Models;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Services;

namespace SS.CMS.Core.StlParser
{
    public partial class ParseContext
    {
        public IConfiguration Configuration { get; }
        public ISettingsManager SettingsManager { get; }
        public ICacheManager CacheManager { get; }
        public IPluginManager PluginManager { get; }
        public IPathManager PathManager { get; }
        public IUrlManager UrlManager { get; }
        public IFileManager FileManager { get; }
        public ITableManager TableManager { get; }
        public ISiteRepository SiteRepository { get; }
        public IChannelRepository ChannelRepository { get; }
        public IUserRepository UserRepository { get; }
        public ITableStyleRepository TableStyleRepository { get; }
        public ITemplateRepository TemplateRepository { get; }
        public ITagRepository TagRepository { get; }
        public IErrorLogRepository ErrorLogRepository { get; }

        public ParseContext(PageInfo pageInfo, IConfiguration configuration, ISettingsManager settingsManager, ICacheManager cacheManager, IPluginManager pluginManager, IPathManager pathManager, IUrlManager urlManager, IFileManager fileManager, ITableManager tableManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IUserRepository userRepository, ITableStyleRepository tableStyleRepository, ITemplateRepository templateRepository, ITagRepository tagRepository, IErrorLogRepository errorLogRepository)
        {
            PageInfo = pageInfo;
            ChannelId = pageInfo.PageChannelId;
            ContentId = pageInfo.PageContentId;

            Configuration = configuration;
            SettingsManager = settingsManager;
            CacheManager = CacheManager;
            PluginManager = pluginManager;
            PathManager = pathManager;
            UrlManager = urlManager;
            FileManager = fileManager;
            TableManager = tableManager;
            SiteRepository = siteRepository;
            ChannelRepository = channelRepository;
            UserRepository = userRepository;
            TableStyleRepository = tableStyleRepository;
            TemplateRepository = templateRepository;
            TagRepository = tagRepository;
            ErrorLogRepository = errorLogRepository;
        }

        //用于clone
        private ParseContext(ParseContext parseContext)
        {
            PageInfo = parseContext.PageInfo.Clone();
            ContextType = parseContext.ContextType;
            ChannelId = parseContext.ChannelId;
            ContentId = parseContext.ContentId;
            _channelInfo = parseContext._channelInfo;
            _contentInfo = parseContext._contentInfo;

            IsInnerElement = parseContext.IsInnerElement;
            IsStlEntity = parseContext.IsStlEntity;
            PageItemIndex = parseContext.PageItemIndex;
            Container = parseContext.Container;
            ContainerClientId = parseContext.ContainerClientId;

            OuterHtml = parseContext.OuterHtml;
            InnerHtml = parseContext.InnerHtml;
            Attributes = parseContext.Attributes;
        }

        public ParseContext Clone(string outerHtml, string innerHtml, NameValueCollection attributes)
        {
            var contextInfo = new ParseContext(this)
            {
                OuterHtml = outerHtml,
                InnerHtml = innerHtml,
                Attributes = attributes
            };
            return contextInfo;
        }

        public ParseContext Clone()
        {
            var contextInfo = new ParseContext(this);
            return contextInfo;
        }

        public ParseContext Clone(EContextType contextType)
        {
            var contextInfo = new ParseContext(this);
            contextInfo.ContextType = contextType;
            return contextInfo;
        }

        public EContextType ContextType { get; set; } = EContextType.Undefined;

        public int ChannelId { get; set; }

        public int ContentId { get; set; }

        public string OuterHtml { get; set; }

        public string InnerHtml { get; set; }

        public NameValueCollection Attributes { get; set; }

        private ChannelInfo _channelInfo;
        public async Task<ChannelInfo> GetChannelInfoAsync()
        {
            if (_channelInfo != null) return _channelInfo;
            if (ChannelId <= 0) return null;
            _channelInfo = await ChannelRepository.GetChannelInfoAsync(SiteId, ChannelId);
            return _channelInfo;
        }

        public ChannelInfo ChannelInfo
        {
            set => _channelInfo = value;
        }

        private ContentInfo _contentInfo;
        public async Task<ContentInfo> GetContentInfoAsync()
        {
            if (_contentInfo != null) return _contentInfo;
            if (ContentId <= 0) return null;
            var channelInfo = await GetChannelInfoAsync();
            _contentInfo = channelInfo.ContentRepository.GetContentInfo(SiteInfo, channelInfo, ContentId);
            return _contentInfo;
        }

        public ContentInfo ContentInfo
        {
            set => _contentInfo = value;
        }

        public bool IsInnerElement { get; set; }

        public bool IsStlEntity { get; set; }

        public int PageItemIndex { get; set; }

        public Container Container { get; set; }

        public string ContainerClientId { get; set; }
    }
}

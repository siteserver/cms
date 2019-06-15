using System.Collections.Specialized;
using Microsoft.Extensions.Configuration;
using SS.CMS.Abstractions.Models;
using SS.CMS.Abstractions.Repositories;
using SS.CMS.Abstractions.Services;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Models;
using SS.CMS.Core.Services;
using SS.CMS.Core.StlParser.Models;

namespace SS.CMS.Core.StlParser
{
    public partial class ParseContext
    {
        public IConfiguration Configuration { get; }
        public ISettingsManager SettingsManager { get; }
        public IPluginManager PluginManager { get; }
        public IPathManager PathManager { get; }
        public IUrlManager UrlManager { get; }
        public IFileManager FileManager { get; }
        public ISiteRepository SiteRepository { get; }
        public IUserRepository UserRepository { get; }
        public ITableStyleRepository TableStyleRepository { get; }
        public ITemplateRepository TemplateRepository { get; }

        public ParseContext(PageInfo pageInfo, IConfiguration configuration, ISettingsManager settingsManager, IPluginManager pluginManager, IPathManager pathManager, IUrlManager urlManager, IFileManager fileManager, ISiteRepository siteRepository, IUserRepository userRepository, ITableStyleRepository tableStyleRepository, ITemplateRepository templateRepository)
        {
            PageInfo = pageInfo;
            ChannelId = pageInfo.PageChannelId;
            ContentId = pageInfo.PageContentId;

            Configuration = configuration;
            SettingsManager = settingsManager;
            PluginManager = pluginManager;
            PathManager = pathManager;
            UrlManager = urlManager;
            FileManager = fileManager;
            SiteRepository = siteRepository;
            UserRepository = userRepository;
            TableStyleRepository = tableStyleRepository;
            TemplateRepository = templateRepository;
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
        public ChannelInfo ChannelInfo
        {
            get
            {
                if (_channelInfo != null) return _channelInfo;
                if (ChannelId <= 0) return null;
                _channelInfo = ChannelManager.GetChannelInfo(SiteId, ChannelId);
                return _channelInfo;
            }
            set { _channelInfo = value; }
        }

        private ContentInfo _contentInfo;
        public ContentInfo ContentInfo
        {
            get
            {
                if (_contentInfo != null) return _contentInfo;
                if (ContentId <= 0) return null;
                _contentInfo = ChannelInfo.ContentRepository.GetContentInfo(SiteInfo, ChannelInfo, ContentId);
                return _contentInfo;
            }
            set { _contentInfo = value; }
        }

        public bool IsInnerElement { get; set; }

        public bool IsStlEntity { get; set; }

        public int PageItemIndex { get; set; }

        public Container Container { get; set; }

        public string ContainerClientId { get; set; }
    }
}

using SS.CMS.Repositories;
using SS.CMS.Services;

namespace SS.CMS.Core.Services
{
    public partial class FileManager : IFileManager
    {
        private readonly ISettingsManager _settingsManager;
        private readonly IUrlManager _urlManager;
        private readonly IPathManager _pathManager;
        private readonly IPluginManager _pluginManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IErrorLogRepository _errorLogRepository;

        public FileManager(ISettingsManager settingsManager, IUrlManager urlManager, IPathManager pathManager, IPluginManager pluginManager, ISiteRepository siteRepository, IChannelRepository channelRepository, ITemplateRepository templateRepository, ITagRepository tagRepository, IErrorLogRepository errorLogRepository)
        {
            _settingsManager = settingsManager;
            _urlManager = urlManager;
            _pathManager = pathManager;
            _pluginManager = pluginManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _templateRepository = templateRepository;
            _tagRepository = tagRepository;
            _errorLogRepository = errorLogRepository;
        }
    }
}
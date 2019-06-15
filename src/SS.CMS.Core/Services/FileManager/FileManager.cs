using SS.CMS.Abstractions.Repositories;
using SS.CMS.Abstractions.Services;

namespace SS.CMS.Core.Services
{
    public partial class FileManager : IFileManager
    {
        private readonly ISettingsManager _settingsManager;
        private readonly IUrlManager _urlManager;
        private readonly IPathManager _pathManager;
        private readonly IPluginManager _pluginManager;
        private readonly ISiteRepository _siteRepository;
        private readonly ITemplateRepository _templateRepository;

        public FileManager(ISettingsManager settingsManager, IUrlManager urlManager, IPathManager pathManager, IPluginManager pluginManager, ISiteRepository siteRepository, ITemplateRepository templateRepository)
        {
            _settingsManager = settingsManager;
            _urlManager = urlManager;
            _pathManager = pathManager;
            _pluginManager = pluginManager;
            _siteRepository = siteRepository;
            _templateRepository = templateRepository;
        }
    }
}
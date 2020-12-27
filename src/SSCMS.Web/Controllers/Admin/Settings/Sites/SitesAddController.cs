using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Core.Utils;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Settings.Sites
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SitesAddController : ControllerBase
    {
        public const string Route = "settings/sitesAdd";
        private const string RouteProcess = "settings/sitesAdd/actions/process";

        private readonly ICacheManager _cacheManager;
        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ICreateManager _createManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IAdministratorRepository _administratorRepository;
        private readonly ITemplateRepository _templateRepository;

        public SitesAddController(ICacheManager cacheManager, ISettingsManager settingsManager, IAuthManager authManager, IPathManager pathManager, ICreateManager createManager, IDatabaseManager databaseManager, ISiteRepository siteRepository, IContentRepository contentRepository, IAdministratorRepository administratorRepository, ITemplateRepository templateRepository)
        {
            _cacheManager = cacheManager;
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _createManager = createManager;
            _databaseManager = databaseManager;
            _siteRepository = siteRepository;
            _contentRepository = contentRepository;
            _administratorRepository = administratorRepository;
            _templateRepository = templateRepository;
        }

        public class GetResult
        {
            public IEnumerable<SiteType> SiteTypes { get; set; }
            public List<SiteTemplateInfo> SiteTemplates { get; set; }
            public bool RootExists { get; set; }
            public List<Cascade<int>> Sites { get; set; }
            public List<string> TableNameList { get; set; }
            public string Guid { get; set; }
        }

        public class SubmitRequest
        {
            public string Guid { get; set; }
            public string SiteType { get; set; }
            public string CreateType { get; set; }
            public string LocalDirectoryName { get; set; }
            public string CloudThemeUserName { get; set; }
            public string CloudThemeName { get; set; }
            public string SiteName { get; set; }
            public bool Root { get; set; }
            public int ParentId { get; set; }
            public string SiteDir { get; set; }
            public TableRule TableRule { get; set; }
            public string TableChoose { get; set; }
            public string TableHandWrite { get; set; }
            public bool IsImportContents { get; set; }
            public bool IsImportTableStyles { get; set; }
        }

        public class ProcessRequest
        {
            public string Guid { get; set; }
        }

        //private static void AddSite(List<KeyValuePair<int, string>> siteList, Site site, Dictionary<int, List<Site>> parentWithChildren, int level)
        //{
        //    if (level > 1) return;
        //    var padding = string.Empty;
        //    for (var i = 0; i < level; i++)
        //    {
        //        padding += "　";
        //    }
        //    if (level > 0)
        //    {
        //        padding += "└ ";
        //    }

        //    if (parentWithChildren.ContainsKey(site.Id))
        //    {
        //        var children = parentWithChildren[site.Id];
        //        siteList.Add(new KeyValuePair<int, string>(site.Id, padding + site.SiteName + $"({children.Count})"));
        //        level++;
        //        foreach (var subSite in children)
        //        {
        //            AddSite(siteList, subSite, parentWithChildren, level);
        //        }
        //    }
        //    else
        //    {
        //        siteList.Add(new KeyValuePair<int, string>(site.Id, padding + site.SiteName));
        //    }
        //}
    }
}

using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class AudioController : ControllerBase
    {
        private const string Route = "cms/material/audio";
        private const string RouteUpdate = "cms/material/audio/actions/update";
        private const string RouteDelete = "cms/material/audio/actions/delete";
        private const string RouteDeleteGroup = "cms/material/audio/actions/deleteGroup";
        private const string RouteDownload = "cms/material/audio/actions/download";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IWxManager _wxManager;
        private readonly IConfigRepository _configRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IMaterialGroupRepository _materialGroupRepository;
        private readonly IMaterialAudioRepository _materialAudioRepository;

        public AudioController(
            ISettingsManager settingsManager,
            IAuthManager authManager,
            IPathManager pathManager,
            IWxManager wxManager,
            IConfigRepository configRepository,
            ISiteRepository siteRepository,
            IMaterialGroupRepository materialGroupRepository,
            IMaterialAudioRepository materialAudioRepository
        )
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _wxManager = wxManager;
            _configRepository = configRepository;
            _siteRepository = siteRepository;
            _materialGroupRepository = materialGroupRepository;
            _materialAudioRepository = materialAudioRepository;
        }

        public class QueryRequest
        {
            public int SiteId { get; set; }
            public string Keyword { get; set; }
            public int GroupId { get; set; }
            public int Page { get; set; }
            public int PerPage { get; set; }
        }

        public class QueryResult
        {
            public bool IsSiteOnly { get; set; }
            public IEnumerable<MaterialGroup> Groups { get; set; }
            public int Count { get; set; }
            public IEnumerable<MaterialAudio> Items { get; set; }
            public string SiteType { get; set; }
        }

        public class CreateRequest : SiteRequest
        {
            public int GroupId { get; set; }
        }

        public class DownloadRequest : SiteRequest
        {
            public int Id { get; set; }
        }

        public class UpdateRequest : SiteRequest
        {
            public int Id { get; set; }

            public string Title { get; set; }

            public int GroupId { get; set; }
        }

        public class DeleteRequest : SiteRequest
        {
            public int Id { get; set; }
        }

        public class DeleteGroupRequest : SiteRequest
        {
            public int Id { get; set; }
        }
    }
}

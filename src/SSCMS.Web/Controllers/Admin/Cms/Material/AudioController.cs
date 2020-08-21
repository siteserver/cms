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
        private const string RouteActionsDeleteGroup = "cms/material/audio/actions/deleteGroup";
        private const string RouteActionsPull = "cms/material/audio/actions/pull";
        private const string RouteActionsDownload = "cms/material/audio/actions/download";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IWxManager _openManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IMaterialGroupRepository _materialGroupRepository;
        private readonly IMaterialAudioRepository _materialAudioRepository;
        private readonly IWxAccountRepository _openAccountRepository;

        public AudioController(ISettingsManager settingsManager, IAuthManager authManager, IPathManager pathManager, IWxManager openManager, ISiteRepository siteRepository, IMaterialGroupRepository materialGroupRepository, IMaterialAudioRepository materialAudioRepository, IWxAccountRepository openAccountRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _openManager = openManager;
            _siteRepository = siteRepository;
            _materialGroupRepository = materialGroupRepository;
            _materialAudioRepository = materialAudioRepository;
            _openAccountRepository = openAccountRepository;
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

        public class PullRequest : SiteRequest
        {
            public int GroupId { get; set; }
        }

        public class PullResult
        {
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
        }
    }
}

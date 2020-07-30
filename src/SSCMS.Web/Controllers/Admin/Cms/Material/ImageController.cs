using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Material
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ImageController : ControllerBase
    {
        private const string Route = "cms/material/image";
        private const string RouteActionsDeleteGroup = "cms/material/image/actions/deleteGroup";
        private const string RouteActionsPull = "cms/material/image/actions/pull";
        private const string RouteActionsDownload = "cms/material/image/actions/download";

        private readonly ISettingsManager _settingsManager;
        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IWxManager _openManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IMaterialGroupRepository _materialGroupRepository;
        private readonly IMaterialImageRepository _materialImageRepository;
        private readonly IWxAccountRepository _openAccountRepository;

        public ImageController(ISettingsManager settingsManager, IAuthManager authManager, IPathManager pathManager, IWxManager openManager, ISiteRepository siteRepository, IMaterialGroupRepository materialGroupRepository, IMaterialImageRepository materialImageRepository, IWxAccountRepository openAccountRepository)
        {
            _settingsManager = settingsManager;
            _authManager = authManager;
            _pathManager = pathManager;
            _openManager = openManager;
            _siteRepository = siteRepository;
            _materialGroupRepository = materialGroupRepository;
            _materialImageRepository = materialImageRepository;
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
            public IEnumerable<MaterialImage> Items { get; set; }
            public bool IsOpen { get; set; }
        }

        public class CreateRequest : SiteRequest
        {
            public int GroupId { get; set; }
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

        public class DownloadRequest : SiteRequest
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

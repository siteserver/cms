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
    public partial class LayerVideoUploadController : ControllerBase
    {
        private const string Route = "cms/material/layerVideoUpload";

        private readonly ISettingsManager _settingsManager;
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IMaterialVideoRepository _materialVideoRepository;

        public LayerVideoUploadController(ISettingsManager settingsManager, IPathManager pathManager, ISiteRepository siteRepository, IMaterialVideoRepository materialVideoRepository)
        {
            _settingsManager = settingsManager;
            _pathManager = pathManager;
            _siteRepository = siteRepository;
            _materialVideoRepository = materialVideoRepository;
        }

        public class GetResult
        {
            public string VideoUploadExtensions { get; set; }
        }

        public class SubmitRequest : SiteRequest
        {
            public int GroupId { get; set; }
        }

        public class SubmitResult
        {
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
            public MaterialVideo Video { get; set; }
        }
    }
}

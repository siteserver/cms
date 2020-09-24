using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SettingsWaterMarkController : ControllerBase
    {
        private const string Route = "cms/settings/settingsWaterMark";
        private const string RouteUpload = "cms/settings/settingsWaterMark/actions/upload";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ISiteRepository _siteRepository;

        public SettingsWaterMarkController(IAuthManager authManager, IPathManager pathManager, ISiteRepository siteRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _siteRepository = siteRepository;
        }

        public class GetResult
        {
            public Site Site { get; set; }
            public IEnumerable<string> Families { get; set; }
            public string ImageUrl { get; set; }
        }

        public class UploadResult
        {
            public string ImageUrl { get; set; }
            public string VirtualUrl { get; set; }
        }

        public class SubmitRequest : SiteRequest
        {
            public bool IsWaterMark { get; set; }
            public int WaterMarkPosition { get; set; }
            public int WaterMarkTransparency { get; set; }
            public int WaterMarkMinWidth { get; set; }
            public int WaterMarkMinHeight { get; set; }
            public bool IsImageWaterMark { get; set; }
            public string WaterMarkFormatString { get; set; }
            public string WaterMarkFontName { get; set; }
            public int WaterMarkFontSize { get; set; }
            public string WaterMarkImagePath { get; set; }
        }
    }
}

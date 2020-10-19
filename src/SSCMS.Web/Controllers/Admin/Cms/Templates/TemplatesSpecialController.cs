using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class TemplatesSpecialController : ControllerBase
    {
        private const string Route = "cms/templates/templatesSpecial";
        private const string RouteId = "cms/templates/templatesSpecial/{siteId:int}/{specialId:int}";
        private const string RouteDownload = "cms/templates/templatesSpecial/actions/download";
        private const string RouteUpload = "cms/templates/templatesSpecial/actions/upload";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ICreateManager _createManager;
        private readonly ISiteRepository _siteRepository;
        private readonly ISpecialRepository _specialRepository;

        public TemplatesSpecialController(IAuthManager authManager, IPathManager pathManager, ICreateManager createManager, ISiteRepository siteRepository, ISpecialRepository specialRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _createManager = createManager;
            _siteRepository = siteRepository;
            _specialRepository = specialRepository;
        }

        public class ListResult
        {
            public List<Special> Specials { get; set; }
            public string SiteUrl { get; set; }
        }

        public class SpecialIdRequest : SiteRequest
        {
            public int SpecialId { get; set; }
        }

        public class DeleteResult
        {
            public List<Special> Specials { get; set; }
        }

        public class GetSpecialResult
        {
            public Special Special { get; set; }
            public string Guid { get; set; }
        }

        public class UploadRequest : SiteRequest
        {
            public string Guid { get; set; }
        }

        public class SubmitRequest : SiteRequest
        {
            public int Id { get; set; }
            public string Guid { get; set; }
            public string Title { get; set; }
            public string Url { get; set; }
            public IEnumerable<string> FileNames { get; set; }
            public bool IsEditOnly { get; set; }
            public bool IsUploadOnly { get; set; }
        }

        public class SubmitResult
        {
            public List<Special> Specials { get; set; }
        }
    }
}

using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Cms.Forms
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class FormDataAddController : ControllerBase
    {
        private const string Route = "cms/forms/formDataAdd";
        
        private const string RouteDeleteFile = "cms/forms/{siteId}/formDataAdd/actions/deleteFile";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IFormManager _formManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IFormRepository _formRepository;
        private readonly IFormDataRepository _formDataRepository;

        public FormDataAddController(IAuthManager authManager, IPathManager pathManager, IFormManager formManager, ISiteRepository siteRepository, IFormRepository formRepository, IFormDataRepository formDataRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _formManager = formManager;
            _siteRepository = siteRepository;
            _formRepository = formRepository;
            _formDataRepository = formDataRepository;
        }

        public class FormRequest : SiteRequest
        {
            public int FormId { get; set; }
        }

        public class GetRequest : FormRequest
        {
            public int DataId { get; set; }
        }

        public class GetResult
        {
            public string SiteUrl { get; set; }
            public List<TableStyle> Styles { get; set; }
            public FormData FormData { get; set; }
        }

        public class DeleteRequest : FormRequest
        {
            public string FileUrl { get; set; }
        }
    }
}

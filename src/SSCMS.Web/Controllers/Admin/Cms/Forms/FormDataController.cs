using System;
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
    public partial class FormDataController : ControllerBase
    {
        private const string Route = "cms/forms/formData";
        private const string RouteExport = "cms/forms/formData/actions/export";
        private const string RouteColumns = "cms/forms/formData/actions/columns";
        private const string RouteImport = "cms/forms/formData/actions/import";
        private const string RouteDelete = "cms/forms/formData/actions/delete";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ISmsManager _smsManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IFormRepository _formRepository;
        private readonly IFormDataRepository _formDataRepository;

        public FormDataController(IAuthManager authManager, IPathManager pathManager, ISmsManager smsManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, IFormRepository formRepository, IFormDataRepository formDataRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _smsManager = smsManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _formRepository = formRepository;
            _formDataRepository = formDataRepository;
        }

        public class FormRequest : SiteRequest
        {
            public int FormId { get; set; }
        }

        public class GetRequest : FormRequest
        {
            public int Page { get; set; }
            public string Keyword { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
        }

        public class GetResult
        {
            public List<FormData> Items { get; set; }
            public int Total { get; set; }
            public int PageSize { get; set; }
            public List<TableStyle> Styles { get; set; }
            public List<string> AllAttributeNames { get; set; }
            public List<string> ListAttributeNames { get; set; }
            public bool IsReply { get; set; }
            public List<ContentColumn> Columns { get; set; }
        }

        public class DeleteRequest : FormRequest
        {
            public int Page { get; set; }
            public List<int> DataIds { get; set; }
        }

        public class ColumnsRequest : FormRequest
        {
            public List<string> AttributeNames { get; set; }
        }

        public class ImportRequest
        {
            public int SiteId { get; set; }
            public int FormId { get; set; }
        }
    }
}

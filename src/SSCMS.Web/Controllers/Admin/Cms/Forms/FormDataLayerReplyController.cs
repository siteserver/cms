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
    public partial class FormDataLayerReplyController : ControllerBase
    {
        private const string Route = "cms/forms/formDataLayerReply";

        private readonly IAuthManager _authManager;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IFormRepository _formRepository;
        private readonly IFormDataRepository _formDataRepository;

        public FormDataLayerReplyController(IAuthManager authManager, IChannelRepository channelRepository, IContentRepository contentRepository, IFormRepository formRepository, IFormDataRepository formDataRepository)
        {
            _authManager = authManager;
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
            public int DataId { get; set; }
        }

        public class GetResult
        {
            public List<TableStyle> Styles { get; set; }
            public List<ContentColumn> Columns { get; set; }
            public FormData FormData { get; set; }
            public List<string> AttributeNames { get; set; }
        }

        public class SubmitRequest : GetRequest
        {
            public string ReplyContent { get; set; }
        }
    }
}

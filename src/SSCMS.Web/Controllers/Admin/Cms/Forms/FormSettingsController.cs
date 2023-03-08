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
    public partial class FormSettingsController : ControllerBase
    {
        private const string Route = "cms/forms/formSettings";

        private readonly IAuthManager _authManager;
        private readonly ISmsManager _smsManager;
        private readonly IMailManager _mailManager;
        private readonly IFormRepository _formRepository;

        public FormSettingsController(IAuthManager authManager, ISmsManager smsManager, IMailManager mailManager, IFormRepository formRepository)
        {
            _authManager = authManager;
            _smsManager = smsManager;
            _mailManager = mailManager;
            _formRepository = formRepository;
        }

        public class FormRequest : SiteRequest
        {
            public int FormId { get; set; }
        }

        public class GetResult
        {
            public Form Form { get; set; }
            public List<TableStyle> Styles { get; set; }
            public List<string> AttributeNames { get; set; }
            public List<string> AdministratorSmsNotifyKeys { get; set; }
            public List<string> UserSmsNotifyKeys { get; set; }
            public bool IsSmsEnabled { get; set; }
            public bool IsMailEnabled { get; set; }
        }

        public class SubmitRequest : FormRequest
        {
            public bool IsClosed { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string SuccessMessage { get; set; }
            public string SuccessCallback { get; set; }
            public bool IsReply { get; set; }
            public bool IsReplyListAll { get; set; }
            public string ListAttributeNames { get; set; }
            public bool IsCaptcha { get; set; }
            public bool IsSms { get; set; }
            public int PageSize { get; set; }
            public bool IsTimeout { get; set; }
            public DateTime TimeToStart { get; set; }
            public DateTime TimeToEnd { get; set; }
            public bool IsAdministratorSmsNotify { get; set; }
            public string AdministratorSmsNotifyTplId { get; set; }
            public string AdministratorSmsNotifyKeys { get; set; }
            public string AdministratorSmsNotifyMobile { get; set; }
            public bool IsAdministratorMailNotify { get; set; }
            public string AdministratorMailTitle { get; set; }
            public string AdministratorMailNotifyAddress { get; set; }
            public bool IsUserSmsNotify { get; set; }
            public string UserSmsNotifyTplId { get; set; }
            public string UserSmsNotifyKeys { get; set; }
            public string UserSmsNotifyMobileName { get; set; }
        }
    }
}

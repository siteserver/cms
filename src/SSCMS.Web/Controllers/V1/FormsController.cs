using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.V1
{
    [ApiController]
    [ApiConventionType(typeof(DefaultApiConventions))]
    [Route(Constants.ApiV1Prefix)]
    public partial class FormsController : ControllerBase
    {
        private const string Route = "forms";
        private const string RouteStyles = "forms/styles";
        private const string RouteSendSms = "forms/actions/sendSms";
        private const string RouteUpload = "forms/actions/upload";

        private readonly ICacheManager _cacheManager;
        private readonly IPathManager _pathManager;
        private readonly ISmsManager _smsManager;
        private readonly IFormManager _formManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IFormRepository _formRepository;
        private readonly IFormDataRepository _formDataRepository;

        public FormsController(ICacheManager cacheManager, IPathManager pathManager, ISmsManager smsManager, IFormManager formManager, ISiteRepository siteRepository, IFormRepository formRepository, IFormDataRepository formDataRepository)
        {
            _cacheManager = cacheManager;
            _pathManager = pathManager;
            _smsManager = smsManager;
            _formManager = formManager;
            _siteRepository = siteRepository;
            _formRepository = formRepository;
            _formDataRepository = formDataRepository;
        }

        public class StylesRequest : SiteRequest
        {
            public int FormId { get; set; }
        }

        public class StylesResult
        {
            public string SiteUrl { get; set; }
            public List<TableStyle> Styles { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string SuccessMessage { get; set; }
            public string SuccessCallback { get; set; }
            public bool IsSms { get; set; }
            public bool IsCaptcha { get; set; }
            public FormData FormData { get; set; }
        }

        public class GetRequest : SiteRequest
        {
            public int FormId { get; set; }
            public int Page { get; set; }
            public string Word { get; set; }
        }

        public class UploadRequest : SiteRequest
        {
            public int FormId { get; set; }
            public string AttributeName { get; set; }
        }

        public class UploadResult
        {
            public string AttributeName { get; set; }
            public string VirtualUrl { get; set; }
            public string FileUrl { get; set; }
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

        public class SendSmsRequest
        {
            public string Mobile { get; set; }
        }

        public class SubmitRequest
        {
            public int SiteId { get; set; }
            public int ChannelId { get; set; }
            public int ContentId { get; set; }
            public int FormId { get; set; }
        }

        private static string GetUploadTokenCacheKey(int formId)
        {
            return $"SSCMS.Web.Controllers.V1.FormsController.Actions.Upload.{formId}";
        }

        private string GetSmsCodeCacheKey(int formId, string mobile)
        {
            return $"SSCMS.Web.Controllers.V1.FormsController.Actions.SendSms.{formId}.{mobile}";
        }
    }
}

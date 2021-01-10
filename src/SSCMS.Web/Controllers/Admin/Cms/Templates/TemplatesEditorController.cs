using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Cms.Templates
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class TemplatesEditorController : ControllerBase
    {
        private const string Route = "cms/templates/templatesEditor";
        private const string RouteCreate = "cms/templates/templatesEditor/actions/create";
        private const string RouteSettings = "cms/templates/templatesEditor/actions/settings";
        private const string RoutePreview = "cms/templates/templatesEditor/actions/preview";
		private const string RouteGetContents = "cms/templates/templatesEditor/actions/getContents";

		private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly ICreateManager _createManager;
        private readonly IParseManager _parseManager;
        private readonly ISiteRepository _siteRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;

        public TemplatesEditorController(IAuthManager authManager, IPathManager pathManager, ICreateManager createManager, IParseManager parseManager, ISiteRepository siteRepository, ITemplateRepository templateRepository, IChannelRepository channelRepository, IContentRepository contentRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _createManager = createManager;
            _parseManager = parseManager;
			_siteRepository = siteRepository;
            _templateRepository = templateRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
        }

		public class SettingsForm
        {
			public int SiteId { get; set; }
			public int TemplateId { get; set; }
			public TemplateType TemplateType { get; set; }
			public string TemplateName { get; set; }
            public string RelatedFileName { get; set; }
            public string CreatedFileFullName { get; set; }
            public string CreatedFileExtName { get; set; }
		}

		public class GetResult
		{
            public SettingsForm Settings { get; set; }
			public string Content { get; set; }
            public Cascade<int> Channels { get; set; }
			public List<int> ChannelIds { get; set; }
			public int ChannelId { get; set; }
            public IEnumerable<KeyValuePair<int, string>> Contents { get; set; }
			public int ContentId { get; set; }
		}

		public class GetContentsRequest : SiteRequest
        {
            public int ChannelId { get; set; }
        }

        public class GetContentsResult
        {
            public IEnumerable<KeyValuePair<int, string>> Contents { get; set; }
			public int ContentId { get; set; }
        }

		public class TemplateRequest : SiteRequest
		{
			public int TemplateId { get; set; }
			public TemplateType TemplateType { get; set; }
		}

        public class SettingsRequest
        {
			public SettingsForm Settings { get; set; }
			public string Content { get; set; }
		}

		public class SettingsResult
        {
            public SettingsForm Settings { get; set; }
		}

		public class SubmitRequest : SiteRequest
		{
			public int TemplateId { get; set; }
            public string Content { get; set; }
		}

        public class PreviewRequest : SiteRequest
        {
            public int TemplateId { get; set; }
            public int ChannelId { get; set; }
            public int ContentId { get; set; }
            public string Content { get; set; }
        }

        public class PreviewResult
		{
            public string BaseUrl { get; set; }
            public string Html { get; set; }
        }

		private string GetTemplateFileExtension(Template template)
		{
			string extension;
			if (template.TemplateType == TemplateType.IndexPageTemplate || template.TemplateType == TemplateType.FileTemplate)
			{
				extension = PathUtils.GetExtension(template.CreatedFileFullName);
			}
			else
			{
				extension = template.CreatedFileExtName;
			}
			return extension;
		}

		private async Task CreatePagesAsync(Template template)
		{
			if (template.TemplateType == TemplateType.FileTemplate)
			{
				await _createManager.CreateFileAsync(template.SiteId, template.Id);
			}
			else if (template.TemplateType == TemplateType.IndexPageTemplate)
			{
				if (template.DefaultTemplate)
				{
					await _createManager.CreateChannelAsync(template.SiteId, template.SiteId);
				}
			}
		}

        private async Task<SettingsForm> GetSettingsAsync(Template templateInfo, Site site)
		{
			var template = new Template
			{
				Id = templateInfo.Id,
				SiteId = site.Id,
				DefaultTemplate = templateInfo.DefaultTemplate,
				TemplateType = templateInfo.TemplateType,
				TemplateName = templateInfo.TemplateName
			};
			if (templateInfo.Id > 0)
			{
				template.CreatedFileExtName = GetTemplateFileExtension(templateInfo);
				template.RelatedFileName = PathUtils.RemoveExtension(templateInfo.RelatedFileName);
				template.CreatedFileFullName = PathUtils.RemoveExtension(templateInfo.CreatedFileFullName);
				template.Content = await _pathManager.GetTemplateContentAsync(site, templateInfo);
			}
			else
			{
				template.RelatedFileName = "T_";
				template.CreatedFileFullName = template.TemplateType == TemplateType.ChannelTemplate ? "index" : "@/";
				template.CreatedFileExtName = ".html";
			}

            return new SettingsForm
            {
                SiteId = site.Id,
                TemplateId = template.Id,
                TemplateType = template.TemplateType,
                TemplateName = template.TemplateName,
                RelatedFileName = template.RelatedFileName,
                CreatedFileFullName = template.CreatedFileFullName,
                CreatedFileExtName = template.CreatedFileExtName
            };
		}
    }
}

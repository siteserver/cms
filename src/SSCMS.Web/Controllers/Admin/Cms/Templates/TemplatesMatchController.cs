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
    public partial class TemplatesMatchController : ControllerBase
    {
        private const string Route = "cms/templates/templatesMatch";
        private const string RouteCreate = "cms/templates/templatesMatch/actions/create";

        private readonly IAuthManager _authManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly ITemplateRepository _templateRepository;

        public TemplatesMatchController(IAuthManager authManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, ITemplateRepository templateRepository)
        {
            _authManager = authManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _templateRepository = templateRepository;
        }

        public class GetResult
        {
            public Cascade<int> Channels { get; set; }
            public IEnumerable<Template> ChannelTemplates { get; set; }
            public IEnumerable<Template> ContentTemplates { get; set; }
        }

        public class MatchRequest
        {
            public int SiteId { get; set; }
            public List<int> ChannelIds { get; set; }
            public bool IsChannelTemplate { get; set; }
            public int TemplateId { get; set; }
        }

        public class CreateRequest
        {
            public int SiteId { get; set; }
            public List<int> ChannelIds { get; set; }
            public bool IsChannelTemplate { get; set; }
            public bool IsChildren { get; set; }
        }

        private async Task CreateChannelTemplateAsync(CreateRequest request)
        {
            var defaultChannelTemplateId = await _templateRepository.GetDefaultTemplateIdAsync(request.SiteId, TemplateType.ChannelTemplate);
            var relatedFileNameList = await _templateRepository.GetRelatedFileNamesAsync(request.SiteId, TemplateType.ChannelTemplate);
            var templateNameList = await _templateRepository.GetTemplateNamesAsync(request.SiteId, TemplateType.ChannelTemplate);

            foreach (var channelId in request.ChannelIds)
            {
                var channelTemplateId = -1;

                var nodeInfo = await _channelRepository.GetAsync(channelId);
                if (nodeInfo.ParentId > 0)
                {
                    channelTemplateId = nodeInfo.ChannelTemplateId;
                }

                if (channelTemplateId != -1 && channelTemplateId != 0 && channelTemplateId != defaultChannelTemplateId)
                {
                    if (await _templateRepository.GetAsync(channelTemplateId) == null)
                    {
                        channelTemplateId = -1;
                    }
                }

                if (channelTemplateId != -1)
                {
                    var templateInfo = new Template
                    {
                        Id = 0,
                        SiteId = request.SiteId,
                        TemplateName = nodeInfo.ChannelName,
                        TemplateType = TemplateType.ChannelTemplate,
                        RelatedFileName = "T_" + nodeInfo.ChannelName + ".html",
                        CreatedFileFullName = "index.html",
                        CreatedFileExtName = ".html",
                        DefaultTemplate = false
                    };

                    if (ListUtils.ContainsIgnoreCase(relatedFileNameList, templateInfo.RelatedFileName))
                    {
                        continue;
                    }
                    if (templateNameList.Contains(templateInfo.TemplateName))
                    {
                        continue;
                    }
                    var insertedTemplateId = await _templateRepository.InsertAsync(templateInfo);
                    if (nodeInfo.ParentId > 0)
                    {
                        nodeInfo.ChannelTemplateId = insertedTemplateId;
                        await _channelRepository.UpdateChannelTemplateIdAsync(nodeInfo);

                        //TemplateManager.UpdateChannelTemplateId(SiteId, channelId, insertedTemplateId);
                        //DataProvider.BackgroundNodeDAO.UpdateChannelTemplateID(channelId, insertedTemplateID);
                    }

                }
            }
        }

        private async Task CreateChannelChildrenTemplateAsync(Site site, CreateRequest request)
        {
            var relatedFileNameList = await _templateRepository.GetRelatedFileNamesAsync(request.SiteId, TemplateType.ChannelTemplate);
            var templateNameList = await _templateRepository.GetTemplateNamesAsync(request.SiteId, TemplateType.ChannelTemplate);
            foreach (var channelId in request.ChannelIds)
            {
                var nodeInfo = await _channelRepository.GetAsync(channelId);

                var templateInfo = new Template
                {
                    Id = 0,
                    SiteId = request.SiteId,
                    TemplateName = nodeInfo.ChannelName + "_下级",
                    TemplateType = TemplateType.ChannelTemplate,
                    RelatedFileName = "T_" + nodeInfo.ChannelName + "_下级.html",
                    CreatedFileFullName = "index.html",
                    CreatedFileExtName = ".html",
                    DefaultTemplate = false
                };

                if (ListUtils.ContainsIgnoreCase(relatedFileNameList, templateInfo.RelatedFileName))
                {
                    continue;
                }
                if (templateNameList.Contains(templateInfo.TemplateName))
                {
                    continue;
                }
                var insertedTemplateId = await _templateRepository.InsertAsync(templateInfo);
                var childChannelIdList = await _channelRepository.GetChannelIdsAsync(site.Id, channelId, ScopeType.Descendant);
                foreach (var childChannelId in childChannelIdList)
                {
                    var childChannelInfo = await _channelRepository.GetAsync(childChannelId);
                    childChannelInfo.ChannelTemplateId = insertedTemplateId;
                    await _channelRepository.UpdateChannelTemplateIdAsync(childChannelInfo);

                    //TemplateManager.UpdateChannelTemplateId(SiteId, childChannelId, insertedTemplateId);
                    //DataProvider.BackgroundNodeDAO.UpdateChannelTemplateID(childChannelId, insertedTemplateID);
                }
            }
        }

        private async Task CreateContentTemplateAsync(CreateRequest request)
        {
            var defaultContentTemplateId = await _templateRepository.GetDefaultTemplateIdAsync(request.SiteId, TemplateType.ContentTemplate);
            var relatedFileNameList = await _templateRepository.GetRelatedFileNamesAsync(request.SiteId, TemplateType.ContentTemplate);
            var templateNameList = await _templateRepository.GetTemplateNamesAsync(request.SiteId, TemplateType.ContentTemplate);
            foreach (var channelId in request.ChannelIds)
            {
                var nodeInfo = await _channelRepository.GetAsync(channelId);

                var contentTemplateId = nodeInfo.ContentTemplateId;

                if (contentTemplateId != 0 && contentTemplateId != defaultContentTemplateId)
                {
                    if (await _templateRepository.GetAsync(contentTemplateId) == null)
                    {
                        contentTemplateId = -1;
                    }
                }

                if (contentTemplateId != -1)
                {
                    var templateInfo = new Template
                    {
                        Id = 0,
                        SiteId = request.SiteId,
                        TemplateName = nodeInfo.ChannelName,
                        TemplateType = TemplateType.ContentTemplate,
                        RelatedFileName = "T_" + nodeInfo.ChannelName + ".html",
                        CreatedFileFullName = "index.html",
                        CreatedFileExtName = ".html",
                        DefaultTemplate = false
                    };
                    if (ListUtils.ContainsIgnoreCase(relatedFileNameList, templateInfo.RelatedFileName))
                    {
                        continue;
                    }
                    if (templateNameList.Contains(templateInfo.TemplateName))
                    {
                        continue;
                    }
                    var insertedTemplateId = await _templateRepository.InsertAsync(templateInfo);

                    var channelInfo = await _channelRepository.GetAsync(channelId);
                    channelInfo.ContentTemplateId = insertedTemplateId;
                    await _channelRepository.UpdateContentTemplateIdAsync(channelInfo);

                    //TemplateManager.UpdateContentTemplateId(SiteId, channelId, insertedTemplateId);
                    //DataProvider.BackgroundNodeDAO.UpdateContentTemplateID(channelId, insertedTemplateID);
                }
            }
        }

        private async Task CreateContentChildrenTemplateAsync(CreateRequest request)
        {
            var relatedFileNameList = await _templateRepository.GetRelatedFileNamesAsync(request.SiteId, TemplateType.ContentTemplate);
            var templateNameList = await _templateRepository.GetTemplateNamesAsync(request.SiteId, TemplateType.ContentTemplate);
            foreach (var channelId in request.ChannelIds)
            {
                var nodeInfo = await _channelRepository.GetAsync(channelId);

                var templateInfo = new Template
                {
                    Id = 0,
                    SiteId = request.SiteId,
                    TemplateName = nodeInfo.ChannelName + "_下级",
                    TemplateType = TemplateType.ContentTemplate,
                    RelatedFileName = "T_" + nodeInfo.ChannelName + "_下级.html",
                    CreatedFileFullName = "index.html",
                    CreatedFileExtName = ".html",
                    DefaultTemplate = false
                };

                if (ListUtils.ContainsIgnoreCase(relatedFileNameList, templateInfo.RelatedFileName))
                {
                    continue;
                }
                if (templateNameList.Contains(templateInfo.TemplateName))
                {
                    continue;
                }
                var insertedTemplateId = await _templateRepository.InsertAsync(templateInfo);
                var childChannelIdList = await _channelRepository.GetChannelIdsAsync(request.SiteId, channelId, ScopeType.Descendant);
                foreach (var childChannelId in childChannelIdList)
                {
                    var childChannelInfo = await _channelRepository.GetAsync(childChannelId);
                    childChannelInfo.ContentTemplateId = insertedTemplateId;
                    await _channelRepository.UpdateContentTemplateIdAsync(childChannelInfo);

                    //TemplateManager.UpdateContentTemplateId(SiteId, childChannelId, insertedTemplateId);
                    //DataProvider.BackgroundNodeDAO.UpdateContentTemplateID(childChannelId, insertedTemplateID);
                }
            }
        }
    }
}

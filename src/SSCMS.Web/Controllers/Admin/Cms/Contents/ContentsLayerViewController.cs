using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Cms.Contents
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class ContentsLayerViewController : ControllerBase
    {
        private const string Route = "cms/contents/contentsLayerView";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly IContentRepository _contentRepository;
        private readonly IContentGroupRepository _contentGroupRepository;
        private readonly IContentTagRepository _contentTagRepository;
        private readonly ITableStyleRepository _tableStyleRepository;

        public ContentsLayerViewController(IAuthManager authManager, IPathManager pathManager, IDatabaseManager databaseManager, ISiteRepository siteRepository, IChannelRepository channelRepository, IContentRepository contentRepository, IContentGroupRepository contentGroupRepository, IContentTagRepository contentTagRepository, ITableStyleRepository tableStyleRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _contentRepository = contentRepository;
            _contentGroupRepository = contentGroupRepository;
            _contentTagRepository = contentTagRepository;
            _tableStyleRepository = tableStyleRepository;
        }

        public class GetRequest : ChannelRequest
        {
            public int ContentId { get; set; }
        }

        public class GetResult
        {
            public Content Content { get; set; }
            public string ChannelName { get; set; }
            public string State { get; set; }
            public List<ContentColumn> Columns { get; set; }
            public string SiteUrl { get; set; }
            public IEnumerable<string> GroupNames { get; set; }
            public IEnumerable<string> TagNames { get; set; }
            public List<ContentColumn> EditorColumns { get; set; }
        }
    }
}

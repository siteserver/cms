using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Cms.Settings
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class SettingsStyleRelatedFieldController : ControllerBase
    {
        private const string Route = "cms/settings/settingsStyleRelatedField";
        private const string RouteImport = "cms/settings/settingsStyleRelatedField/actions/import";
        private const string RouteExport = "cms/settings/settingsStyleRelatedField/actions/export";
        private const string RouteItems = "cms/settings/settingsStyleRelatedField/items";
        private const string RouteItemsOrder = "cms/settings/settingsStyleRelatedField/items/actions/order";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly ISiteRepository _siteRepository;
        private readonly IRelatedFieldRepository _relatedFieldRepository;
        private readonly IRelatedFieldItemRepository _relatedFieldItemRepository;

        public SettingsStyleRelatedFieldController(IAuthManager authManager, IPathManager pathManager, IDatabaseManager databaseManager, ISiteRepository siteRepository, IRelatedFieldRepository relatedFieldRepository, IRelatedFieldItemRepository relatedFieldItemRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _siteRepository = siteRepository;
            _relatedFieldRepository = relatedFieldRepository;
            _relatedFieldItemRepository = relatedFieldItemRepository;
        }

        public class ItemsRequest
        {
            public int SiteId { get; set; }
            public int RelatedFieldId { get; set; }
        }

        public class ItemsResult
        {
            public List<Cascade<int>> Tree { get; set; }
        }

        public class ItemsAddRequest : SiteRequest
        {
            public int RelatedFieldId { get; set; }
            public int ParentId { get; set; }
            public List<KeyValuePair<string, string>> Items { get; set; }
        }

        public class ItemsEditRequest : SiteRequest
        {
            public int RelatedFieldId { get; set; }
            public int Id { get; set; }
            public string Label { get; set; }
            public string Value { get; set; }
        }

        public class ItemsOrderRequest : SiteRequest
        {
            public int RelatedFieldId { get; set; }
            public int Id { get; set; }
            public bool Up { get; set; }
        }

        public class ItemsDeleteRequest : SiteRequest
        {
            public int RelatedFieldId { get; set; }
            public int Id { get; set; }
        }

        public class DeleteRequest : SiteRequest
        {
            public int RelatedFieldId { get; set; }
        }
    }
}

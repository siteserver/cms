using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Common.TableStyle
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LayerEditorController : ControllerBase
    {
        private const string Route = "common/tableStyle/layerEditor";

        private readonly IAuthManager _authManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly ITableStyleRepository _tableStyleRepository;
        private readonly IRelatedFieldRepository _relatedFieldRepository;

        public LayerEditorController(IAuthManager authManager, IDatabaseManager databaseManager, ITableStyleRepository tableStyleRepository, IRelatedFieldRepository relatedFieldRepository)
        {
            _authManager = authManager;
            _databaseManager = databaseManager;
            _tableStyleRepository = tableStyleRepository;
            _relatedFieldRepository = relatedFieldRepository;
        }

        public class GetRequest
        {
            public int SiteId { get; set; }
            public string TableName { get; set; }
            public string AttributeName { get; set; }
            public string RelatedIdentities { get; set; }
        }

        public class GetResult
        {
            public IEnumerable<KeyValuePair<InputType, string>> InputTypes { get; set; }
            public List<RelatedField> RelatedFields { get; set; }
            public SubmitRequest Form { get; set; }
        }

        public class SubmitRequest
        {
            public string TableName { get; set; }
            public string AttributeName { get; set; }
            public string RelatedIdentities { get; set; }
            public bool IsRapid { get; set; }
            public string RapidValues { get; set; }
            public int Taxis { get; set; }
            public string DisplayName { get; set; }
            public string HelpText { get; set; }
            public InputType InputType { get; set; }
            public string DefaultValue { get; set; }
            public bool Horizontal { get; set; }
            public List<InputStyleItem> Items { get; set; }
            public int Height { get; set; }
            public int? RelatedFieldId { get; set; }
            public string CustomizeCode { get; set; }
        }
    }
}

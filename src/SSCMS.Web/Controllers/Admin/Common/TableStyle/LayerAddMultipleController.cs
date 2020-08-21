using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Enums;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Common.TableStyle
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LayerAddMultipleController : ControllerBase
    {
        private const string Route = "common/tableStyle/layerAddMultiple";

        private readonly IAuthManager _authManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly ITableStyleRepository _tableStyleRepository;

        public LayerAddMultipleController(IAuthManager authManager, IDatabaseManager databaseManager, ITableStyleRepository tableStyleRepository)
        {
            _authManager = authManager;
            _databaseManager = databaseManager;
            _tableStyleRepository = tableStyleRepository;
        }

        public class GetResult
        {
            public IEnumerable<KeyValuePair<InputType, string>> InputTypes { get; set; }
            public List<Style> Styles { get; set; }
        }

        public class SubmitRequest
        {
            public string TableName { get; set; }
            public string RelatedIdentities { get; set; }
            public List<Style> Styles { get; set; }
        }

        public class Style
        {
            public string AttributeName { get; set; }
            public string DisplayName { get; set; }
            public InputType InputType { get; set; }
        }
    }
}

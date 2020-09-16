using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Common.TableStyle
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LayerValidateController : ControllerBase
    {
        private const string Route = "common/tableStyle/layerValidate";

        private readonly IAuthManager _authManager;
        private readonly ITableStyleRepository _tableStyleRepository;

        public LayerValidateController(IAuthManager authManager, ITableStyleRepository tableStyleRepository)
        {
            _authManager = authManager;
            _tableStyleRepository = tableStyleRepository;
        }

        public class GetRequest
        {
            public string TableName { get; set; }
            public string AttributeName { get; set; }
            public string RelatedIdentities { get; set; }
        }

        public class GetResult
        {
            public IEnumerable<Select<string>> Options { get; set; }
            public IEnumerable<InputStyleRule> Rules { get; set; }
        }

        public class SubmitRequest
        {
            public string TableName { get; set; }
            public string AttributeName { get; set; }
            public string RelatedIdentities { get; set; }
            public List<InputStyleRule> Rules { get; set; }
        }
    }
}

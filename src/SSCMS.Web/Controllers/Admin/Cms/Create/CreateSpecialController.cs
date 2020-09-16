using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Cms.Create
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class CreateSpecialController : ControllerBase
    {
        private const string Route = "cms/create/createSpecial";

        private readonly IAuthManager _authManager;
        private readonly ICreateManager _createManager;
        private readonly ISiteRepository _siteRepository;
        private readonly ISpecialRepository _specialRepository;

        public CreateSpecialController(IAuthManager authManager, ICreateManager createManager, ISiteRepository siteRepository, ISpecialRepository specialRepository)
        {
            _authManager = authManager;
            _createManager = createManager;
            _siteRepository = siteRepository;
            _specialRepository = specialRepository;
        }

        public class GetResult
        {
            public IEnumerable<Special> Specials { get; set; }
        }

        public class CreateRequest : SiteRequest
        {
            public IEnumerable<int> SpecialIds { get; set; }
        }

        
        

        
    }
}

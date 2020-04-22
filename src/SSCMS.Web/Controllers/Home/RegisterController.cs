using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Repositories;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home
{
    [OpenApiIgnore]
    [Route(Constants.ApiHomePrefix)]
    public partial class RegisterController : ControllerBase
    {
        private const string Route = "register";

        private readonly IConfigRepository _configRepository;
        private readonly ITableStyleRepository _tableStyleRepository;
        private readonly IUserGroupRepository _userGroupRepository;

        public RegisterController(IConfigRepository configRepository, ITableStyleRepository tableStyleRepository, IUserGroupRepository userGroupRepository)
        {
            _configRepository = configRepository;
            _tableStyleRepository = tableStyleRepository;
            _userGroupRepository = userGroupRepository;
        }

        [HttpGet, Route(Route)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var config = await _configRepository.GetAsync();

            return new GetResult
            {
                Config = config,
                Styles = await _tableStyleRepository.GetUserStyleListAsync(),
                Groups = await _userGroupRepository.GetUserGroupListAsync()
            };
        }
    }
}

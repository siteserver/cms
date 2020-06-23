using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Dto;
using SSCMS.Extensions;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.User)]
    [Route(Constants.ApiHomePrefix)]
    public partial class PasswordController : ControllerBase
    {
        private const string Route = "password";

        private readonly IAuthManager _authManager;
        private readonly IUserRepository _userRepository;

        public PasswordController(IAuthManager authManager, IUserRepository userRepository)
        {
            _authManager = authManager;
            _userRepository = userRepository;
        }

        [HttpGet, Route(Route)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var user = await _authManager.GetUserAsync();

            return new GetResult
            {
                User = user
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody]SubmitRequest request)
        {
            var password = request.Password;
            var (isValid, errorMessage) = await _userRepository.ChangePasswordAsync(_authManager.UserId, password);
            if (!isValid)
            {
                return this.Error($"更改密码失败：{errorMessage}");
            }

            await _authManager.AddUserLogAsync("修改密码");

            return new BoolResult
            {
                Value = true
            };
        }
    }
}

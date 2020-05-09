using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Core.Extensions;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Home
{
    [OpenApiIgnore]
    [Route(Constants.ApiHomePrefix)]
    public partial class RegisterController : ControllerBase
    {
        private const string Route = "register";
        private const string RouteCaptcha = "register/captcha";
        private const string RouteCheckCaptcha = "register/captcha/actions/check";

        private readonly ISettingsManager _settingsManager;
        private readonly IConfigRepository _configRepository;
        private readonly ITableStyleRepository _tableStyleRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserGroupRepository _userGroupRepository;

        public RegisterController(ISettingsManager settingsManager, IConfigRepository configRepository, ITableStyleRepository tableStyleRepository, IUserRepository userRepository, IUserGroupRepository userGroupRepository)
        {
            _settingsManager = settingsManager;
            _configRepository = configRepository;
            _tableStyleRepository = tableStyleRepository;
            _userRepository = userRepository;
            _userGroupRepository = userGroupRepository;
        }

        [HttpGet, Route(Route)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<GetResult>> Get()
        {
            var config = await _configRepository.GetAsync();
            if (config.IsHomeClosed) return this.Error("对不起，用户中心已被禁用！");
            if (!config.IsUserRegistrationAllowed) return this.Error("对不起，系统已禁止新用户注册！");

            var userStyles = await _tableStyleRepository.GetUserStyleListAsync();
            var styles = userStyles
                .Where(x => StringUtils.ContainsIgnoreCase(config.UserRegistrationAttributes, x.AttributeName))
                .Select(x => new InputStyle(x));

            return new GetResult
            {
                IsUserRegistrationGroup = config.IsUserRegistrationGroup,
                IsHomeAgreement = config.IsHomeAgreement,
                HomeAgreementHtml = config.HomeAgreementHtml,
                Styles = styles,
                Groups = await _userGroupRepository.GetUserGroupListAsync()
            };
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<BoolResult>> Submit([FromBody]User request)
        {
            var ipAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            var (user, errorMessage) = await _userRepository.InsertAsync(request, request.Password, ipAddress);
            if (user == null)
            {
                return this.Error($"用户注册失败：{errorMessage}");
            }

            return new BoolResult
            {
                Value = user.Checked
            };
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Settings.Logs
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class LogsUserController : ControllerBase
    {
        private const string Route = "settings/logsUser";
        private const string RouteExport = "settings/logsUser/actions/export";
        private const string RouteDelete = "settings/logsUser/actions/delete";

        private readonly IAuthManager _authManager;
        private readonly IPathManager _pathManager;
        private readonly IUserRepository _userRepository;
        private readonly ILogRepository _logRepository;

        public LogsUserController(IAuthManager authManager, IPathManager pathManager, IUserRepository userRepository, ILogRepository logRepository)
        {
            _authManager = authManager;
            _pathManager = pathManager;
            _userRepository = userRepository;
            _logRepository = logRepository;
        }

        public class SearchRequest : PageRequest
        {
            public string UserName { get; set; }
            public string Keyword { get; set; }
            public string DateFrom { get; set; }
            public string DateTo { get; set; }
        }

        public async Task<PageResult<Log>> GetResultsAsync(SearchRequest request)
        {
            var userId = 0;
            if (!string.IsNullOrEmpty(request.UserName))
            {
                var user = await _userRepository.GetByUserNameAsync(request.UserName);
                if (user == null)
                {
                    return new PageResult<Log>
                    {
                        Items = new List<Log>(),
                        Count = 0,
                    };
                }
                userId = user.Id;
            }

            var count = await _logRepository.GetUserLogsCountAsync(userId, request.Keyword, request.DateFrom, request.DateTo);
            var userLogs = await _logRepository.GetUserLogsAsync(userId, request.Keyword, request.DateFrom, request.DateTo, request.Offset, request.Limit);
            var logs = new List<Log>();

            foreach (var log in userLogs)
            {
                var user = await _userRepository.GetByUserIdAsync(log.UserId);
                if (user == null) continue;

                var userName = _userRepository.GetDisplay(user);
                log.Set("userName", userName);
                log.Set("userGuid", user.Guid);
                logs.Add(log);
            }

            return new PageResult<Log>
            {
                Items = logs,
                Count = count
            };
        }
    }
}

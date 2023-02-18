using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using NSwag.Annotations;
using SSCMS.Configuration;
using SSCMS.Enums;
using SSCMS.Repositories;
using SSCMS.Services;

namespace SSCMS.Web.Controllers.Admin.Clouds
{
    [OpenApiIgnore]
    [Authorize(Roles = Types.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class BackupController : ControllerBase
    {
        private const string Route = "clouds/backup";
        private const string RouteRestore = "clouds/backup/actions/restore";
        private const string RouteGetRestoreProgress = "clouds/backup/actions/getRestoreProgress";
        private const string RouteRestart = "clouds/backup/actions/restart";

        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IAuthManager _authManager;
        private readonly ICloudManager _cloudManager;
        private readonly ITaskManager _taskManager;
        private readonly IConfigRepository _configRepository;
        private readonly IScheduledTaskRepository _scheduledTaskRepository;

        public BackupController(IHostApplicationLifetime hostApplicationLifetime, IAuthManager authManager, ICloudManager cloudManager, ITaskManager taskManager, IConfigRepository configRepository, IScheduledTaskRepository scheduledTaskRepository)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _authManager = authManager;
            _cloudManager = cloudManager;
            _taskManager = taskManager;
            _configRepository = configRepository;
            _scheduledTaskRepository = scheduledTaskRepository;
        }

        public class GetResult
        {
            public CloudType CloudType { get; set; }
            public bool IsCloudBackup { get; set; }
        }

        public class SubmitRequest
        {
            public bool IsCloudBackup { get; set; }
        }

        public class RestoreRequest
        {
            public string BackupId { get; set; }
        }

        public class GetRestoreProgressRequest
        {
            public string RestoreId { get; set; }
        }
    }
}

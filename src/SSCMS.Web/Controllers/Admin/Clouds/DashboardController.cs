﻿using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public partial class DashboardController : ControllerBase
    {
        private const string Route = "clouds/dashboard";
        private const string RouteDisconnect = "clouds/dashboard/actions/disconnect";

        private readonly IAuthManager _authManager;
        private readonly ICloudManager _cloudManager;

        public DashboardController(
            IAuthManager authManager, 
            ICloudManager cloudManager
        )
        {
            _authManager = authManager;
            _cloudManager = cloudManager;
        }

        public class SubmitRequest
        {
            public CloudType CloudType { get; set; }
            public DateTime ExpirationDate { get; set; }
        }
    }
}

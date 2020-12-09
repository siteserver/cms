using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSCMS.Enums;
using SSCMS.Utils;
using SSCMS.Core.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Analysis
{
    public partial class AnalysisAdminLoginController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromBody] GetRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsAnalysisAdminLogin))
            {
                return Unauthorized();
            }

            var lowerDate = TranslateUtils.ToDateTime(request.DateFrom);
            var higherDate = TranslateUtils.ToDateTime(request.DateTo, DateTime.Now);

            var successStats = await _statRepository.GetStatsAsync(lowerDate, higherDate, StatType.AdminLoginSuccess);
            var failureStats = await _statRepository.GetStatsAsync(lowerDate, higherDate, StatType.AdminLoginFailure);

            var getStats = new List<GetStat>();
            var totalDays = (higherDate - lowerDate).TotalDays;
            for (var i = 0; i <= totalDays; i++)
            {
                var date = lowerDate.AddDays(i).ToString("M-d");

                var success = successStats.FirstOrDefault(x => x.CreatedDate.HasValue && x.CreatedDate.Value.ToString("M-d") == date);
                var failure = failureStats.FirstOrDefault(x => x.CreatedDate.HasValue && x.CreatedDate.Value.ToString("M-d") == date);

                getStats.Add(new GetStat
                {
                    Date = date,
                    Success = success?.Count ?? 0,
                    Failure = failure?.Count ?? 0
                });
            }

            var days = getStats.Select(x => x.Date).ToList();
            var successCount = getStats.Select(x => x.Success).ToList();
            var failureCount = getStats.Select(x => x.Failure).ToList();

            return new GetResult
            {
                Days = days,
                SuccessCount = successCount,
                FailureCount = failureCount
            };
        }
    }
}

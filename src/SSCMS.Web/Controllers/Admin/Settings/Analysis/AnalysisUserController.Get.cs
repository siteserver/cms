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
    public partial class AnalysisUserController
    {
        [HttpPost, Route(Route)]
        public async Task<ActionResult<GetResult>> Get([FromBody] GetRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(MenuUtils.AppPermissions.SettingsAnalysisUser))
            {
                return Unauthorized();
            }

            var lowerDate = TranslateUtils.ToDateTime(request.DateFrom);
            var higherDate = TranslateUtils.ToDateTime(request.DateTo, DateTime.Now);

            var registerStats = await _statRepository.GetStatsAsync(lowerDate, higherDate, StatType.UserRegister);
            var loginStats = await _statRepository.GetStatsAsync(lowerDate, higherDate, StatType.UserLogin);

            var getStats = new List<GetStat>();
            var totalDays = (higherDate - lowerDate).TotalDays;
            for (var i = 0; i <= totalDays; i++)
            {
                var date = lowerDate.AddDays(i).ToString("M-d");

                var register = registerStats.FirstOrDefault(x => x.CreatedDate.HasValue && x.CreatedDate.Value.ToString("M-d") == date);
                var login = loginStats.FirstOrDefault(x => x.CreatedDate.HasValue && x.CreatedDate.Value.ToString("M-d") == date);

                getStats.Add(new GetStat
                {
                    Date = date,
                    Register = register?.Count ?? 0,
                    Login = login?.Count ?? 0
                });
            }

            var days = getStats.Select(x => x.Date).ToList();
            var registerCount = getStats.Select(x => x.Register).ToList();
            var loginCount = getStats.Select(x => x.Login).ToList();

            return new GetResult
            {
                Days = days,
                RegisterCount = registerCount,
                LoginCount = loginCount
            };
        }
    }
}
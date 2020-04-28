using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NSwag.Annotations;
using SSCMS.Core.Utils;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Web.Controllers.Admin.Settings.Analysis
{
    [OpenApiIgnore]
    [Authorize(Roles = AuthTypes.Roles.Administrator)]
    [Route(Constants.ApiAdminPrefix)]
    public partial class AnalysisAdminLoginController : ControllerBase
    {
        private const string Route = "settings/analysisAdminLogin";

        private readonly IAuthManager _authManager;
        private readonly ILogRepository _logRepository;

        public AnalysisAdminLoginController(IAuthManager authManager, ILogRepository logRepository)
        {
            _authManager = authManager;
            _logRepository = logRepository;
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<QueryResult>> List([FromBody] QueryRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsAnalysisAdminLogin))
            {
                return Unauthorized();
            }

            var dateFrom = TranslateUtils.ToDateTime(request.DateFrom);
            var dateTo = TranslateUtils.ToDateTime(request.DateTo, DateTime.Now);
            var xType = TranslateUtils.ToEnum(request.XType, AnalysisType.Day);

            var trackingDayDictionary = _logRepository.GetAdminLoginDictionaryByDate(dateFrom, dateTo,
                request.XType, Constants.AdminLogin);
            var adminNumDictionaryName =
                await _logRepository.GetAdminLoginDictionaryByNameAsync(dateFrom, dateTo, Constants.AdminLogin);

            var count = 0;
            var dictionaryDay = new Dictionary<int, int>();
            var maxAdminNum = 0;
            if (xType == AnalysisType.Day)
            {
                count = 30;
            }
            else if (xType == AnalysisType.Month)
            {
                count = 12;
            }
            else if (xType == AnalysisType.Year)
            {
                count = 10;
            }

            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            for (var i = 0; i < count; i++)
            {
                var datetime = now.AddDays(-i);
                if (xType == AnalysisType.Day)
                {
                    now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                    datetime = now.AddDays(-i);
                }
                else if (xType == AnalysisType.Month)
                {
                    now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                    datetime = now.AddMonths(-i);
                }
                else if (xType == AnalysisType.Year)
                {
                    now = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0);
                    datetime = now.AddYears(-i);
                }

                var accessNum = 0;
                if (trackingDayDictionary.ContainsKey(datetime))
                {
                    accessNum = trackingDayDictionary[datetime];
                }
                dictionaryDay.Add(count - i, accessNum);
                if (accessNum > maxAdminNum)
                {
                    maxAdminNum = accessNum;
                }
            }

            var result = new QueryResult
            {
                DateX = new List<string>(),
                DateY = new List<string>(),
                NameX = new List<string>(),
                NameY = new List<string>()
            };

            for (var i = 1; i <= count; i++)
            {
                result.DateX.Add(GetGraphicX(i, xType, count));
                result.DateY.Add(GetGraphicY(i, dictionaryDay, count));
            }

            foreach (var key in adminNumDictionaryName.Keys)
            {
                result.NameX.Add(key);
                result.NameY.Add(GetGraphicYUser(adminNumDictionaryName, key));
            }

            return result;
        }

        private static string GetGraphicX(int index, AnalysisType xType, int count)
        {
            var xNum = 0;
            var datetime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            if (xType == AnalysisType.Day)
            {
                datetime = datetime.AddDays(-(count - index));
                xNum = datetime.Day;
            }
            else if (xType == AnalysisType.Month)
            {
                datetime = datetime.AddMonths(-(count - index));
                xNum = datetime.Month;
            }
            else if (xType == AnalysisType.Year)
            {
                datetime = datetime.AddYears(-(count - index));
                xNum = datetime.Year;
            }
            return xNum.ToString();
        }

        private static string GetGraphicY(int index, Dictionary<int, int> dictionaryDay, int count)
        {
            if (index <= 0 || index > count || !dictionaryDay.ContainsKey(index)) return string.Empty;
            var accessNum = dictionaryDay[index];
            return accessNum.ToString();
        }

        private static string GetGraphicYUser(Dictionary<string, int> adminNumDictionaryName, string key)
        {
            if (string.IsNullOrEmpty(key) || !adminNumDictionaryName.ContainsKey(key)) return string.Empty;
            var accessNum = adminNumDictionaryName[key];
            return accessNum.ToString();
        }
    }
}

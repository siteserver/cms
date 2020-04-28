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
    public partial class AnalysisUserController : ControllerBase
    {
        private const string Route = "settings/analysisUser";

        private readonly IAuthManager _authManager;
        private readonly IUserRepository _userRepository;

        public AnalysisUserController(IAuthManager authManager, IUserRepository userRepository)
        {
            _authManager = authManager;
            _userRepository = userRepository;
        }

        [HttpPost, Route(Route)]
        public async Task<ActionResult<QueryResult>> List([FromBody] QueryRequest request)
        {
            if (!await _authManager.HasAppPermissionsAsync(AuthTypes.AppPermissions.SettingsAnalysisUser))
            {
                return Unauthorized();
            }

            var dateFrom = TranslateUtils.ToDateTime(request.DateFrom);
            var dateTo = TranslateUtils.ToDateTime(request.DateTo, DateTime.Now);
            var xType = TranslateUtils.ToEnum(request.XType, AnalysisType.Day);

            var trackingDayDictionary = _userRepository.GetTrackingDictionary(dateFrom, dateTo, request.XType);

            var count = 0;
            var userNumDict = new Dictionary<int, int>();
            var maxUserNum = 0;
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
                userNumDict[count - i] = accessNum;
                if (accessNum > maxUserNum)
                {
                    maxUserNum = accessNum;
                }
            }

            var result = new QueryResult
            {
                DateX = new List<string>(),
                DateY = new List<string>()
            };

            for (var i = 1; i <= count; i++)
            {
                result.DateX.Add(GetGraphicX(i, xType, count));
                result.DateY.Add(GetGraphicY(i, userNumDict, count));
            }

            return result;
        }

        private string GetGraphicX(int index, AnalysisType xType, int count)
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

        private string GetGraphicY(int index, Dictionary<int, int> userNumDict, int count)
        {
            if (index <= 0 || index > count) return string.Empty;
            var accessNum = userNumDict[index];
            return accessNum.ToString();
        }
    }
}

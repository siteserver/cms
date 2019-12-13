using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Settings.Analysis
{
    
    [RoutePrefix("pages/settings/analysisAdminLogin")]
    public partial class PagesAnalysisAdminLoginController : ApiController
    {
        private const string Route = "";

        [HttpPost, Route(Route)]
        public async Task<QueryResult> List([FromBody] QueryRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            await auth.CheckSettingsPermissions(Request, Constants.AppPermissions.SettingsAnalysisAdminLogin);

            var dateFrom = TranslateUtils.ToDateTime(request.DateFrom);
            var dateTo = TranslateUtils.ToDateTime(request.DateTo, DateTime.Now);
            var xType = EStatictisXTypeUtils.GetEnumType(request.XType);

            var trackingDayDictionary = DataProvider.LogRepository.GetAdminLoginDictionaryByDate(dateFrom, dateTo,
                EStatictisXTypeUtils.GetValue(xType), Constants.AdminLogin);
            var adminNumDictionaryName =
                DataProvider.LogRepository.GetAdminLoginDictionaryByName(dateFrom, dateTo, Constants.AdminLogin);

            var count = 0;
            var dictionaryDay = new Dictionary<int, int>();
            var maxAdminNum = 0;
            if (xType == EStatictisXType.Day)
            {
                count = 30;
            }
            else if (xType == EStatictisXType.Month)
            {
                count = 12;
            }
            else if (xType == EStatictisXType.Year)
            {
                count = 10;
            }

            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            for (var i = 0; i < count; i++)
            {
                var datetime = now.AddDays(-i);
                if (xType == EStatictisXType.Day)
                {
                    now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                    datetime = now.AddDays(-i);
                }
                else if (xType == EStatictisXType.Month)
                {
                    now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                    datetime = now.AddMonths(-i);
                }
                else if (xType == EStatictisXType.Year)
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

        private static string GetGraphicX(int index, EStatictisXType xType, int count)
        {
            var xNum = 0;
            var datetime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            if (xType == EStatictisXType.Day)
            {
                datetime = datetime.AddDays(-(count - index));
                xNum = datetime.Day;
            }
            else if (xType == EStatictisXType.Month)
            {
                datetime = datetime.AddMonths(-(count - index));
                xNum = datetime.Month;
            }
            else if (xType == EStatictisXType.Year)
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

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.Abstractions;
using SiteServer.CMS.Core;
using SiteServer.CMS.Context.Enumerations;
using SiteServer.CMS.Extensions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Settings.Analysis
{

    [RoutePrefix("pages/settings/analysisSite")]
    public partial class PagesAnalysisSiteController : ApiController
    {
        private const string Route = "";

        private const string YTypeNew = "YType_New";
        private const string YTypeUpdate = "YType_Update";

        [HttpPost, Route(Route)]
        public async Task<QueryResult> List([FromBody] QueryRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            if (!auth.IsAdminLoggin ||
                !await auth.AdminPermissionsImpl.HasSystemPermissionsAsync(Constants.AppPermissions.SettingsAnalysisSite))
            {
                return Request.Unauthorized<QueryResult>();
            }

            var dateFrom = TranslateUtils.ToDateTime(request.DateFrom);
            var dateTo = TranslateUtils.ToDateTime(request.DateTo, DateTime.Now);
            var result = new QueryResult
            {
                NewX = new List<string>(),
                NewY = new List<int>(),
                UpdateX = new List<string>(),
                UpdateY = new List<int>(),
                Items = new List<QueryResultItem>()
            };

            //总数
            var horizontalDict = new Dictionary<int, int>();
            var verticalDict = new Dictionary<string, int>();
            //x
            var xDict = new Dictionary<int, string>();
            //y
            var yDictNew = new Dictionary<int, int>();
            var yDictUpdate = new Dictionary<int, int>();

            var siteIdList = await DataProvider.SiteRepository.GetSiteIdListOrderByLevelAsync();

            foreach (var siteId in siteIdList)
            {
                var site = await DataProvider.SiteRepository.GetAsync(siteId);

                var key = site.Id;
                //x轴信息
                SetXDict(key, site.SiteName, xDict);
                //y轴信息
                SetYDict(key, await DataProvider.ContentRepository.GetCountOfContentAddAsync(site.TableName, site.Id, site.Id, EScopeType.All, dateFrom, dateTo, string.Empty, ETriState.All), YTypeNew, yDictNew, yDictUpdate, verticalDict, horizontalDict);
                SetYDict(key, await DataProvider.ContentRepository.GetCountOfContentUpdateAsync(site.TableName, site.Id, site.Id, EScopeType.All, dateFrom, dateTo, string.Empty), YTypeUpdate, yDictNew, yDictUpdate, verticalDict, horizontalDict);

                var item = new QueryResultItem
                {
                    SiteId = siteId,
                    SiteName = site.SiteName,
                    SiteUrl = await PageUtility.GetSiteUrlAsync(site, false),
                    AddCount = GetYDict(siteId, YTypeNew, yDictNew, yDictUpdate),
                    UpdateCount = GetYDict(siteId, YTypeUpdate, yDictNew, yDictUpdate),
                    TotalCount = GetHorizontal(siteId, horizontalDict)
                };
                result.Items.Add(item);

                var yValueNew = GetYDict(key, YTypeNew, yDictNew, yDictUpdate);
                var yValueUpdate = GetYDict(key, YTypeUpdate, yDictNew, yDictUpdate);

                if (yValueNew != 0)
                {
                    result.NewX.Add(GetXDict(key, xDict));
                    result.NewY.Add(yValueNew);
                    
                }
                if (yValueUpdate != 0)
                {
                    result.UpdateX.Add(GetXDict(key, xDict));
                    result.UpdateY.Add(yValueUpdate);
                }
            }

            return result;
        }

        /// <summary>
        /// 设置x轴数据
        /// </summary>
        private void SetXDict(int siteId, string siteName, Dictionary<int, string> xDict)
        {
            if (!xDict.ContainsKey(siteId))
            {
                xDict.Add(siteId, siteName);
            }
        }

        /// <summary>
        /// 获取x轴数据
        /// </summary>
        private string GetXDict(int siteId, Dictionary<int, string> xDict)
        {
            return xDict.ContainsKey(siteId) ? xDict[siteId] : string.Empty;
        }

        /// <summary>
        /// 设置y轴数据
        /// </summary>
        private void SetYDict(int siteId, int value, string yType, Dictionary<int, int> yDictNew,
            Dictionary<int, int> yDictUpdate, Dictionary<string, int> verticalDict, Dictionary<int, int> horizontalDict)
        {
            switch (yType)
            {
                case YTypeNew:
                    if (!yDictNew.ContainsKey(siteId))
                    {
                        yDictNew.Add(siteId, value);
                    }
                    else
                    {
                        var num = TranslateUtils.ToInt(yDictNew[siteId].ToString());
                        yDictNew[siteId] = num + value;
                    }

                    SetVertical(YTypeNew, value, verticalDict);
                    break;
                case YTypeUpdate:
                    if (!yDictUpdate.ContainsKey(siteId))
                    {
                        yDictUpdate.Add(siteId, value);
                    }
                    else
                    {
                        var num = TranslateUtils.ToInt(yDictUpdate[siteId].ToString());
                        yDictUpdate[siteId] = num + value;
                    }

                    SetVertical(YTypeUpdate, value, verticalDict);
                    break;
            }

            SetHorizontal(siteId, value, horizontalDict);
        }

        /// <summary>
        /// 获取y轴数据
        /// </summary>
        private int GetYDict(int siteId, string yType, Dictionary<int, int> yDictNew, Dictionary<int, int> yDictUpdate)
        {
            switch (yType)
            {
                case YTypeNew:
                    if (yDictNew.ContainsKey(siteId))
                    {
                        var num = yDictNew[siteId];
                        return num;
                    }

                    return 0;
                case YTypeUpdate:
                    if (yDictUpdate.ContainsKey(siteId))
                    {
                        var num = yDictUpdate[siteId];
                        return num;
                    }

                    return 0;

                default:
                    return 0;
            }
        }

        /// <summary>
        /// 设置y总数
        /// </summary>
        private void SetHorizontal(int siteId, int num, Dictionary<int, int> horizontalDict)
        {
            if (!horizontalDict.ContainsKey(siteId))
            {
                horizontalDict[siteId] = num;
            }
            else
            {
                var totalNum = horizontalDict[siteId];
                horizontalDict[siteId] = totalNum + num;
            }
        }

        /// <summary>
        /// 获取y总数
        /// </summary>
        private int GetHorizontal(int siteId, Dictionary<int, int> horizontalDict)
        {
            if (horizontalDict.ContainsKey(siteId))
            {
                var num = horizontalDict[siteId];
                return (num == 0) ? 0 : num;
            }

            return 0;
        }

        /// <summary>
        /// 设置type总数
        /// </summary>
        private void SetVertical(string type, int num, Dictionary<string, int> verticalDict)
        {
            if (!verticalDict.ContainsKey(type))
            {
                verticalDict[type] = num;
            }
            else
            {
                var totalNum = verticalDict[type];
                verticalDict[type] = totalNum + num;
            }
        }
    }
}

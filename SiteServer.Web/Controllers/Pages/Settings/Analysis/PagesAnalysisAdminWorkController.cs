using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Web.Http;
using SiteServer.CMS.Core;
using SiteServer.Abstractions;
using SiteServer.CMS.Repositories;

namespace SiteServer.API.Controllers.Pages.Settings.Analysis
{
    
    [RoutePrefix("pages/settings/analysisAdminWork")]
    public partial class PagesAnalysisAdminWorkController : ApiController
    {
        private const string Route = "";

        private const string YTypeNew = "YType_New";
        private const string YTypeUpdate = "YType_Update";

        [HttpPost, Route(Route)]
        public async Task<QueryResult> List([FromBody] QueryRequest request)
        {
            var auth = await AuthenticatedRequest.GetAuthAsync();
            await auth.CheckSettingsPermissions(Request, Constants.SettingsPermissions.Analysis);

            var siteId = request.SiteId;
            var dateFrom = TranslateUtils.ToDateTime(request.DateFrom);
            var dateTo = TranslateUtils.ToDateTime(request.DateTo, DateTime.Now);
            var result = new QueryResult
            {
                NewX = new List<string>(),
                NewY = new List<string>(),
                UpdateY = new List<string>(),
                Items = new List<QueryResultItem>()
            };

            var siteIdList = await DataProvider.SiteRepository.GetSiteIdListOrderByLevelAsync();
            if (siteId == 0 && siteIdList.Count > 0)
            {
                siteId = siteIdList[0];
            }

            var site = await DataProvider.SiteRepository.GetAsync(siteId);
            if (site != null)
            {
                var rows = DataProvider.ContentRepository.GetDataSetOfAdminExcludeRecycle(site.TableName, siteId, dateFrom, dateTo);
                if (rows != null)
                {
                    var userNameList = new List<string>();
                    var xHashtableUser = new Hashtable();
                    var yHashtableUserNew = new Hashtable();
                    var yHashtableUserUpdate = new Hashtable();
                    var horizontalHashtableUser = new Hashtable();
                    var verticalHashtableUser = new Hashtable();

                    foreach (DataRow dr in rows)
                    {
                        var item = new QueryResultItem
                        {
                            UserName = dr["userName"].ToString(),
                            AddCount = TranslateUtils.ToInt(dr["addCount"].ToString()),
                            UpdateCount = TranslateUtils.ToInt(dr["updateCount"].ToString())
                        };
                        result.Items.Add(item);

                        SetXHashtableUser(item.UserName, item.UserName, xHashtableUser, userNameList);
                        SetYHashtableUser(item.UserName, item.AddCount, YTypeNew, yHashtableUserNew, yHashtableUserUpdate, verticalHashtableUser, horizontalHashtableUser);
                        SetYHashtableUser(item.UserName, item.UpdateCount, YTypeUpdate, yHashtableUserNew, yHashtableUserUpdate, verticalHashtableUser, horizontalHashtableUser);
                    }

                    var xArrayNewList = new List<string>();
                    var yArrayNewList = new List<string>();
                    var yArrayUpdateList = new List<string>();

                    foreach (var key in userNameList)
                    {
                        var yValueNew = GetYHashtableUser(key, YTypeNew, yHashtableUserNew, yHashtableUserUpdate);
                        var yValueUpdate = GetYHashtableUser(key, YTypeUpdate, yHashtableUserNew, yHashtableUserUpdate);

                        xArrayNewList.Add(GetXHashtableUser(key, xHashtableUser));
                        yArrayNewList.Add(yValueNew);
                        yArrayUpdateList.Add(yValueUpdate);
                    }

                    result.NewX = xArrayNewList;
                    result.NewY = yArrayNewList;
                    result.UpdateY = yArrayUpdateList;
                }
            }

            result.SiteOptions = await DataProvider.SiteRepository.GetSiteOptionsAsync(0);
            result.SiteId = siteId;

            return result;
        }

        private void SetXHashtableUser(string userName, string siteName, Hashtable xHashtableUser, List<string> userNameList)
        {
            if (!xHashtableUser.ContainsKey(userName))
            {
                xHashtableUser.Add(userName, siteName);
            }
            if (!userNameList.Contains(userName))
            {
                userNameList.Add(userName);
            }
            userNameList.Sort();
            userNameList.Reverse();
        }

        private string GetXHashtableUser(string userName, Hashtable xHashtableUser)
        {
            return xHashtableUser.ContainsKey(userName) ? xHashtableUser[userName].ToString() : string.Empty;
        }

        private void SetYHashtableUser(string userName, int value, string yType, Hashtable yHashtableUserNew, Hashtable yHashtableUserUpdate, Hashtable verticalHashtableUser, Hashtable horizontalHashtableUser)
        {
            switch (yType)
            {
                case YTypeNew:
                    if (!yHashtableUserNew.ContainsKey(userName))
                    {
                        yHashtableUserNew.Add(userName, value);
                    }
                    else
                    {
                        var num = TranslateUtils.ToInt(yHashtableUserNew[userName].ToString());
                        yHashtableUserNew[userName] = num + value;
                    }
                    SetVerticalUser(YTypeNew, value, verticalHashtableUser);
                    break;
                case YTypeUpdate:
                    if (!yHashtableUserUpdate.ContainsKey(userName))
                    {
                        yHashtableUserUpdate.Add(userName, value);
                    }
                    else
                    {
                        var num = TranslateUtils.ToInt(yHashtableUserUpdate[userName].ToString());
                        yHashtableUserUpdate[userName] = num + value;
                    }
                    SetVerticalUser(YTypeUpdate, value, verticalHashtableUser);
                    break;
            }
            SetHorizontalUser(userName, value, horizontalHashtableUser);
        }

        private string GetYHashtableUser(string userName, string yType, Hashtable yHashtableUserNew, Hashtable yHashtableUserUpdate)
        {
            switch (yType)
            {
                case YTypeNew:
                    if (yHashtableUserNew.ContainsKey(userName))
                    {
                        var num = TranslateUtils.ToInt(yHashtableUserNew[userName].ToString());
                        return num.ToString();
                    }
                    return "0";
                case YTypeUpdate:
                    if (yHashtableUserUpdate.ContainsKey(userName))
                    {
                        var num = TranslateUtils.ToInt(yHashtableUserUpdate[userName].ToString());
                        return num.ToString();
                    }
                    return "0";

                default:
                    return "0";
            }
        }

        private void SetHorizontalUser(string userName, int num, Hashtable horizontalHashtableUser)
        {
            if (horizontalHashtableUser[userName] == null)
            {
                horizontalHashtableUser[userName] = num;
            }
            else
            {
                var totalNum = (int)horizontalHashtableUser[userName];
                horizontalHashtableUser[userName] = totalNum + num;
            }
        }

        private void SetVerticalUser(string type, int num, Hashtable verticalHashtableUser)
        {
            if (verticalHashtableUser[type] == null)
            {
                verticalHashtableUser[type] = num;
            }
            else
            {
                var totalNum = (int)verticalHashtableUser[type];
                verticalHashtableUser[type] = totalNum + num;
            }
        }
    }
}

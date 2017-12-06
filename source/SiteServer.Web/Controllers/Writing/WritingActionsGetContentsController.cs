using System;
using System.Collections.Generic;
using System.Web.Http;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.API.Core;
using SiteServer.API.Model;
using SiteServer.CMS.Controllers.Writing;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Writing
{
    [RoutePrefix("api")]
    public class WritingActionsGetContentsController : ApiController
    {
        [HttpPost, Route(ActionsGetContents.Route)]
        public IHttpActionResult Main()
        {
            try
            {
                var body = new RequestBody();
                if (!body.IsUserLoggin) return Unauthorized();

                var publishmentSystemId = body.GetPostInt("publishmentSystemId");
                var nodeId = body.GetPostInt("nodeId");

                var searchType = PageUtils.FilterSqlAndXss(body.GetPostString("searchType"));
                var keyword = PageUtils.FilterSqlAndXss(body.GetPostString("keyword"));
                var dateFrom = PageUtils.FilterSqlAndXss(body.GetPostString("dateFrom"));
                var dateTo = PageUtils.FilterSqlAndXss(body.GetPostString("dateTo"));
                var page = body.GetPostInt("page");

                var user = new User(body.UserInfo);
                var groupInfo = UserGroupManager.GetGroupInfo(user.GroupId);
                var adminUserName = groupInfo.Additional.WritingAdminUserName;

                var nodeIdList = new List<int> {nodeId};

                var writingNodeInfoList = PublishmentSystemManager.GetWritingNodeInfoList(adminUserName, publishmentSystemId);
                foreach (var writingNodeInfo in writingNodeInfoList)
                {
                    if (StringUtils.In(writingNodeInfo.ParentsPath, nodeId.ToString()))
                    {
                        nodeIdList.Add(writingNodeInfo.NodeId);
                    }
                }

                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, nodeId);
                var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);
                var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, nodeInfo);
                var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(publishmentSystemId, nodeId);

                var sqlString = DataProvider.ContentDao.GetWritingSelectCommend(user.UserName, tableName, publishmentSystemId, nodeIdList, searchType, keyword, dateFrom, dateTo);

                var results = new List<Dictionary<string, object>>();
                var sqlPager = new SqlPager
                {
                    ItemsPerPage = 20,
                    SelectCommand = sqlString,
                    OrderByString = ETaxisTypeUtils.GetOrderByString(tableStyle, ETaxisType.OrderByAddDateDesc)
                };

                sqlPager.DataBind(page);

                if (sqlPager.TotalCount > 0)
                {
                    foreach (System.Data.DataRowView row in sqlPager.PagedDataSource.DataSource)
                    {
                        var contentInfo = new ContentInfo(row);
                        results.Add(ContentUtility.ContentToDictionary(contentInfo, tableStyle, tableName, relatedIdentities));
                    }
                }

                return Ok(new
                {
                    Results = results,
                    TotalPage = sqlPager.TotalPages
                });
            }
            catch (Exception ex)
            {
                //return InternalServerError(ex);
                return InternalServerError(new Exception("程序错误"));
            }
        }
    }
}

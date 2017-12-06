using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Http;
using BaiRong.Core;
using BaiRong.Core.Model.Attributes;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Text;
using SiteServer.CMS.Controllers.Writing;
using SiteServer.CMS.Core;

namespace SiteServer.API.Controllers.Writing
{
    [RoutePrefix("api")]
    public class WritingActionsEditContentController : ApiController
    {
        [HttpPost, Route(ActionsEditContent.Route)]
        public IHttpActionResult Main()
        {
            try
            {
                var body = new RequestBody();
                if (!body.IsUserLoggin) return Unauthorized();

                var publishmentSystemId = body.GetPostInt("publishmentSystemId");
                var nodeId = body.GetPostInt("nodeId");
                var id = body.GetPostInt("id");

                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, nodeId);
                var tableName = NodeManager.GetTableName(publishmentSystemInfo, nodeInfo);
                var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, nodeInfo);
                var relatedIdentities = RelatedIdentities.GetChannelRelatedIdentities(publishmentSystemId, nodeId);

                var contentInfo = DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, id);

                var postCollection = body.GetPostCollection();
                var extendImageUrl = ContentAttribute.GetExtendAttributeName(BackgroundContentAttribute.ImageUrl);
                if (postCollection.AllKeys.Contains(StringUtils.LowerFirst(extendImageUrl)))
                {
                    postCollection[extendImageUrl] = postCollection[StringUtils.LowerFirst(extendImageUrl)];
                }
                //var postCollection2 = body.GetPostCollection(true);
                //var postCollection = new NameValueCollection();
                //foreach (string key in postCollection1)
                //{
                //    if (!postCollection.AllKeys.Contains(key))
                //    {
                //        postCollection.Add(key, postCollection1[key]);
                //    }
                //}
                //foreach (string key in postCollection2)
                //{
                //    if (!postCollection.AllKeys.Contains(key))
                //    {
                //        postCollection.Add(key, postCollection2[key]);
                //    }
                //}

                InputTypeParser.AddValuesToAttributes(tableStyle, tableName, publishmentSystemInfo, relatedIdentities, postCollection, contentInfo.Attributes, ContentAttribute.HiddenAttributes);

                contentInfo.LastEditDate = DateTime.Now;
                contentInfo.IsChecked = false;

                DataProvider.ContentDao.Update(tableName, publishmentSystemInfo, contentInfo);

                LogUtils.AddUserLog(body.UserName, EUserActionType.WritingEdit, contentInfo.Title);

                return Ok(new { });
            }
            catch (Exception ex)
            {
                //return InternalServerError(ex);
                return InternalServerError(new Exception("程序错误"));
            }
        }
    }
}

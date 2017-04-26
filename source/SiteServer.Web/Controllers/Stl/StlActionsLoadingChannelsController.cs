using System.Text;
using System.Web;
using System.Web.Http;
using BaiRong.Core;
using SiteServer.CMS.Controllers.Stl;
using SiteServer.CMS.Core;
using SiteServer.CMS.StlParser.StlElement;

namespace SiteServer.API.Controllers.Stl
{
    [RoutePrefix("api")]
    public class StlActionsLoadingChannelsController : ApiController
    {
        [HttpPost, Route(ActionsLoadingChannels.Route)]
        public void Main()
        {
            var builder = new StringBuilder();

            try
            {
                var form = HttpContext.Current.Request.Form;
                var publishmentSystemId = TranslateUtils.ToInt(form["publishmentSystemID"]);
                var parentId = TranslateUtils.ToInt(form["parentID"]);
                var target = form["target"];
                var isShowTreeLine = TranslateUtils.ToBool(form["isShowTreeLine"]);
                var isShowContentNum = TranslateUtils.ToBool(form["isShowContentNum"]);
                var currentFormatString = form["currentFormatString"];
                var topNodeId = TranslateUtils.ToInt(form["topNodeID"]);
                var topParentsCount = TranslateUtils.ToInt(form["topParentsCount"]);
                var currentNodeId = TranslateUtils.ToInt(form["currentNodeID"]);

                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                var nodeIdList = DataProvider.NodeDao.GetNodeIdListByParentId(publishmentSystemId, parentId);

                foreach (int nodeId in nodeIdList)
                {
                    var nodeInfo = NodeManager.GetNodeInfo(publishmentSystemId, nodeId);

                    builder.Append(StlTree.GetChannelRowHtml(publishmentSystemInfo, nodeInfo, target, isShowTreeLine, isShowContentNum, TranslateUtils.DecryptStringBySecretKey(currentFormatString), topNodeId, topParentsCount, currentNodeId));
                }
            }
            catch
            {
                // ignored
            }

            HttpContext.Current.Response.Write(builder);
            HttpContext.Current.Response.End();
        }
    }
}

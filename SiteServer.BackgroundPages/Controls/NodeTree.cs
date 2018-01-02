using System;
using System.Text;
using System.Web.UI;
using BaiRong.Core;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Plugin;

namespace SiteServer.BackgroundPages.Controls
{
    public class NodeTree : Control
    {
        private PublishmentSystemInfo _publishmentSystemInfo;

        protected override void Render(HtmlTextWriter writer)
        {
            var builder = new StringBuilder();

            var context = new RequestContext();

            var publishmentSystemId = int.Parse(Page.Request.QueryString["PublishmentSystemID"]);
            _publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var scripts = ChannelLoading.GetScript(_publishmentSystemInfo, ELoadingType.ContentTree, null);
            builder.Append(scripts);
            if (Page.Request.QueryString["PublishmentSystemID"] != null)
            {
                var nodeIdList = DataProvider.NodeDao.GetNodeIdListByParentId(_publishmentSystemInfo.PublishmentSystemId, 0);
                foreach (var nodeId in nodeIdList)
                {
                    var nodeInfo = NodeManager.GetNodeInfo(_publishmentSystemInfo.PublishmentSystemId, nodeId);
                    var enabled = AdminUtility.IsOwningNodeId(context.AdminName, nodeInfo.NodeId);
                    if (!enabled)
                    {
                        if (!AdminUtility.IsHasChildOwningNodeId(context.AdminName, nodeInfo.NodeId)) continue;
                    }

                    builder.Append(ChannelLoading.GetChannelRowHtml(_publishmentSystemInfo, nodeInfo, enabled, ELoadingType.ContentTree, null, context.AdminName));
                }
            }
            writer.Write(builder);
        }
    }
}
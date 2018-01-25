using System;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using System.Collections.Generic;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageChannelDelete : BasePageCms
    {
        public Literal LtlPageTitle;
		public RadioButtonList RblRetainFiles;
        public Button BtnDelete;

        private bool _deleteContents;
        private readonly List<string> _nodeNameList = new List<string>();

        public string ReturnUrl { get; private set; }

        public static string GetRedirectUrl(int siteId, string returnUrl)
        {
            return PageUtils.GetCmsUrl(siteId, nameof(PageChannelDelete), new NameValueCollection
            {
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId", "ReturnUrl");
            ReturnUrl = StringUtils.ValueFromUrl(Body.GetQueryString("ReturnUrl"));
            _deleteContents = Body.GetQueryBool("DeleteContents");

            if (IsPostBack) return;

            var nodeIdList = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("ChannelIDCollection"));
            nodeIdList.Sort();
            nodeIdList.Reverse();
            foreach (var nodeId in nodeIdList)
            {
                if (nodeId == SiteId) continue;
                if (!HasChannelPermissions(nodeId, ConfigManager.Permissions.Channel.ChannelDelete)) continue;

                var nodeInfo = ChannelManager.GetChannelInfo(SiteId, nodeId);
                var displayName = nodeInfo.ChannelName;
                if (nodeInfo.ContentNum > 0)
                {
                    displayName += $"({nodeInfo.ContentNum})";
                }
                _nodeNameList.Add(displayName);
            }

            if (_nodeNameList.Count == 0)
            {
                BtnDelete.Enabled = false;
            }
            else
            {
                if (_deleteContents)
                {
                    LtlPageTitle.Text = "删除内容";
                    InfoMessage(
                        $"此操作将会删除栏目“{TranslateUtils.ObjectCollectionToString(_nodeNameList)}”下的所有内容，确认吗？");
                }
                else
                {
                    LtlPageTitle.Text = "删除栏目";
                    InfoMessage(
                        $"此操作将会删除栏目“{TranslateUtils.ObjectCollectionToString(_nodeNameList)}”及包含的下级栏目，确认吗？");
                }
            }
        }

        public void Delete_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            try
            {
                var nodeIdList = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("ChannelIDCollection"));
                nodeIdList.Sort();
                nodeIdList.Reverse();

                var nodeIdArrayList = new List<int>();
                foreach (var nodeId in nodeIdList)
                {
                    if (nodeId == SiteId) continue;
                    if (HasChannelPermissions(nodeId, ConfigManager.Permissions.Channel.ChannelDelete))
                    {
                        nodeIdArrayList.Add(nodeId);
                    }
                }

                var builder = new StringBuilder();
                foreach (var nodeId in nodeIdArrayList)
                {
                    builder.Append(ChannelManager.GetChannelName(SiteId, nodeId)).Append(",");
                }

                if (builder.Length > 0)
                {
                    builder.Length -= 1;
                }

                if (_deleteContents)
                {
                    SuccessMessage(bool.Parse(RblRetainFiles.SelectedValue) == false
                        ? "成功删除内容以及生成页面！"
                        : "成功删除内容，生成页面未被删除！");

                    foreach (var nodeId in nodeIdArrayList)
                    {
                        var tableName = ChannelManager.GetTableName(SiteInfo, nodeId);
                        var contentIdList = DataProvider.ContentDao.GetContentIdList(tableName, nodeId);
                        DirectoryUtility.DeleteContents(SiteInfo, nodeId, contentIdList);
                        DataProvider.ContentDao.TrashContents(SiteId, tableName, contentIdList);
                    }

                    Body.AddSiteLog(SiteId, "清空栏目下的内容", $"栏目:{builder}");
                }
                else
                {
                    if (bool.Parse(RblRetainFiles.SelectedValue) == false)
                    {
                        DirectoryUtility.DeleteChannels(SiteInfo, nodeIdArrayList);
                        SuccessMessage("成功删除栏目以及相关生成页面！");
                    }
                    else
                    {
                        SuccessMessage("成功删除栏目，相关生成页面未被删除！");
                    }

                    foreach (var nodeId in nodeIdArrayList)
                    {
                        var tableName = ChannelManager.GetTableName(SiteInfo, nodeId);
                        DataProvider.ContentDao.TrashContentsByChannelId(SiteId, tableName, nodeId);
                        DataProvider.ChannelDao.Delete(SiteId, nodeId);
                    }

                    Body.AddSiteLog(SiteId, "删除栏目", $"栏目:{builder}");
                }

                AddWaitAndRedirectScript(ReturnUrl);
            }
            catch (Exception ex)
            {
                FailMessage(ex, _deleteContents ? "删除内容失败！" : "删除栏目失败！");

                LogUtils.AddSystemErrorLog(ex);
            }
        }
    }
}

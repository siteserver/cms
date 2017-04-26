using System;
using System.Collections;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using System.Collections.Generic;
using BaiRong.Core.Text;

namespace SiteServer.BackgroundPages.Cms
{
	public class PageChannelDelete : BasePageCms
    {
        public Literal ltlPageTitle;
		public RadioButtonList RetainFiles;
        public Button Delete;

        private bool _deleteContents;
        private string _returnUrl;
        private readonly ArrayList _nodeNameArrayList = new ArrayList();

        public static string GetRedirectUrl(int publishmentSystemId, string returnUrl)
        {
            return PageUtils.GetCmsUrl(nameof(PageChannelDelete), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"ReturnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID", "ReturnUrl");
            _returnUrl = StringUtils.ValueFromUrl(Body.GetQueryString("ReturnUrl"));
            _deleteContents = Body.GetQueryBool("DeleteContents");

			if (!IsPostBack)
			{
                BreadCrumb(AppManager.Cms.LeftMenu.IdContent, "删除栏目", string.Empty);

                var nodeIDList = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("ChannelIDCollection"));
                nodeIDList.Sort();
                nodeIDList.Reverse();
                foreach (var nodeID in nodeIDList)
				{
                    if (nodeID == PublishmentSystemId) continue;
                    if (HasChannelPermissions(nodeID, AppManager.Cms.Permission.Channel.ChannelDelete))
					{
                        var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, nodeID);
                        var displayName = nodeInfo.NodeName;
                        if (nodeInfo.ContentNum > 0)
                        {
                            displayName += $"({nodeInfo.ContentNum})";
                        }
                        _nodeNameArrayList.Add(displayName);
					}
				}
                if (_nodeNameArrayList.Count == 0)
                {
                    Delete.Enabled = false;
                }
                else
                {
                    if (_deleteContents)
                    {
                        ltlPageTitle.Text = "删除内容";
                        InfoMessage(
                            $"此操作将会删除栏目“{TranslateUtils.ObjectCollectionToString(_nodeNameArrayList)}”下的所有内容，确认吗？");
                    }
                    else
                    {
                        ltlPageTitle.Text = "删除栏目";
                        InfoMessage(
                            $"此操作将会删除栏目“{TranslateUtils.ObjectCollectionToString(_nodeNameArrayList)}”及包含的下级栏目，确认吗？");
                    }
                }
			}
		}

        public void Delete_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
				try
				{
					var nodeIDList = TranslateUtils.StringCollectionToIntList(Body.GetQueryString("ChannelIDCollection"));
                    nodeIDList.Sort();
                    nodeIDList.Reverse();

					var nodeIDArrayList = new List<int>();
                    foreach (var nodeID in nodeIDList)
					{
                        if (nodeID == PublishmentSystemId) continue;
                        if (HasChannelPermissions(nodeID, AppManager.Cms.Permission.Channel.ChannelDelete))
						{
							nodeIDArrayList.Add(nodeID);
						}
					}

                    var builder = new StringBuilder();
                    foreach (int nodeID in nodeIDArrayList)
                    {
                        builder.Append(NodeManager.GetNodeName(PublishmentSystemId, nodeID)).Append(",");
                    }

                    if (builder.Length > 0)
                    {
                        builder.Length -= 1;
                    }

                    if (_deleteContents)
                    {
                        if (bool.Parse(RetainFiles.SelectedValue) == false)
                        {
                            SuccessMessage("成功删除内容以及生成页面！");
                        }
                        else
                        {
                            SuccessMessage("成功删除内容，生成页面未被删除！");
                        }

                        foreach (int nodeID in nodeIDArrayList)
                        {
                            var tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeID);
                            var contentIdList = BaiRongDataProvider.ContentDao.GetContentIdList(tableName, nodeID);
                            DirectoryUtility.DeleteContents(PublishmentSystemInfo, nodeID, contentIdList);
                            DataProvider.ContentDao.TrashContents(PublishmentSystemId, tableName, contentIdList);
                        }

                        Body.AddSiteLog(PublishmentSystemId, "清空栏目下的内容", $"栏目:{builder}");
                    }
                    else
                    {
                        if (bool.Parse(RetainFiles.SelectedValue) == false)
                        {
                            DirectoryUtility.DeleteChannels(PublishmentSystemInfo, nodeIDArrayList);
                            SuccessMessage("成功删除栏目以及相关生成页面！");
                        }
                        else
                        {
                            SuccessMessage("成功删除栏目，相关生成页面未被删除！");
                        }

                        foreach (int nodeID in nodeIDArrayList)
                        {
                            try
                            {
                                var tableName = NodeManager.GetTableName(PublishmentSystemInfo, nodeID);
                                DataProvider.ContentDao.TrashContentsByNodeId(PublishmentSystemId, tableName, nodeID);
                            }
                            catch { }
                            DataProvider.NodeDao.Delete(nodeID);
                        }

                        Body.AddSiteLog(PublishmentSystemId, "删除栏目", $"栏目:{builder}");
                    }

                    AddWaitAndRedirectScript(_returnUrl);
				}
				catch (Exception ex)
				{
                    if (_deleteContents)
                    {
                        FailMessage(ex, "删除内容失败！");
                    }
                    else
                    {
                        FailMessage(ex, "删除栏目失败！");
                    }

                    LogUtils.AddErrorLog(ex);
				}
			}
		}

        public string ReturnUrl => _returnUrl;
	}
}

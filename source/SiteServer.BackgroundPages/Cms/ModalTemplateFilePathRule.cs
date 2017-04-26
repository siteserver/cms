using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.Model.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalTemplateFilePathRule : BasePageCms
    {
        public PlaceHolder phFilePath;
        public TextBox tbFilePath;
        public TextBox tbChannelFilePathRule;
        public TextBox tbContentFilePathRule;

        public Button btnCreateChannelRule;
        public Button btnCreateContentRule;

		private int _nodeId;

        public static string GetOpenWindowString(int publishmentSystemId, int nodeId)
        {
            return PageUtils.GetOpenWindowString("页面命名规则",
                PageUtils.GetCmsUrl(nameof(ModalTemplateFilePathRule), new NameValueCollection
                {
                    {"PublishmentSystemID", publishmentSystemId.ToString()},
                    {"NodeID", nodeId.ToString()}
                }));
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID", "NodeID");
            _nodeId = Body.GetQueryInt("NodeID");

			if (!IsPostBack)
			{
                var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, _nodeId);

                if (nodeInfo.NodeType == ENodeType.BackgroundPublishNode || nodeInfo.LinkType == ELinkType.LinkToFirstChannel || nodeInfo.LinkType == ELinkType.LinkToFirstContent || nodeInfo.LinkType == ELinkType.LinkToLastAddChannel || nodeInfo.LinkType == ELinkType.NoLink)
                {
                    phFilePath.Visible = false;
                }

                var showPopWinString = ModalFilePathRule.GetOpenWindowString(PublishmentSystemId, _nodeId, true, tbChannelFilePathRule.ClientID);
                btnCreateChannelRule.Attributes.Add("onclick", showPopWinString);

                showPopWinString = ModalFilePathRule.GetOpenWindowString(PublishmentSystemId, _nodeId, false, tbContentFilePathRule.ClientID);
                btnCreateContentRule.Attributes.Add("onclick", showPopWinString);

                if (string.IsNullOrEmpty(nodeInfo.FilePath))
                {
                    tbFilePath.Text = PageUtility.GetInputChannelUrl(PublishmentSystemInfo, nodeInfo);
                }
                else
                {
                    tbFilePath.Text = nodeInfo.FilePath;
                }

                if (string.IsNullOrEmpty(nodeInfo.ChannelFilePathRule))
                {
                    tbChannelFilePathRule.Text = PathUtility.GetChannelFilePathRule(PublishmentSystemInfo, _nodeId);
                }
                else
                {
                    tbChannelFilePathRule.Text = nodeInfo.ChannelFilePathRule;
                }

                if (string.IsNullOrEmpty(nodeInfo.ContentFilePathRule))
                {
                    tbContentFilePathRule.Text = PathUtility.GetContentFilePathRule(PublishmentSystemInfo, _nodeId);
                }
                else
                {
                    tbContentFilePathRule.Text = nodeInfo.ContentFilePathRule;
                }
            }
		}

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isSuccess = false;

            try
            {
                var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, _nodeId);

                var filePath = nodeInfo.FilePath;

                if (phFilePath.Visible)
                {
                    tbFilePath.Text = tbFilePath.Text.Trim();
                    if (!string.IsNullOrEmpty(tbFilePath.Text) && !StringUtils.EqualsIgnoreCase(filePath, tbFilePath.Text))
                    {
                        if (!DirectoryUtils.IsDirectoryNameCompliant(tbFilePath.Text))
                        {
                            FailMessage("栏目页面路径不符合系统要求！");
                            return;
                        }

                        if (PathUtils.IsDirectoryPath(tbFilePath.Text))
                        {
                            tbFilePath.Text = PageUtils.Combine(tbFilePath.Text, "index.html");
                        }

                        var filePathArrayList = DataProvider.NodeDao.GetAllFilePathByPublishmentSystemId(PublishmentSystemId);
                        filePathArrayList.AddRange(DataProvider.TemplateMatchDao.GetAllFilePathByPublishmentSystemId(PublishmentSystemId));
                        if (filePathArrayList.IndexOf(tbFilePath.Text) != -1)
                        {
                            FailMessage("栏目修改失败，栏目页面路径已存在！");
                            return;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(tbChannelFilePathRule.Text))
                {
                    var filePathRule = tbChannelFilePathRule.Text.Replace("|", string.Empty);
                    if (!DirectoryUtils.IsDirectoryNameCompliant(filePathRule))
                    {
                        FailMessage("栏目页面命名规则不符合系统要求！");
                        return;
                    }
                    if (PathUtils.IsDirectoryPath(filePathRule))
                    {
                        FailMessage("栏目页面命名规则必须包含生成文件的后缀！");
                        return;
                    }
                }

                if (!string.IsNullOrEmpty(tbContentFilePathRule.Text))
                {
                    var filePathRule = tbContentFilePathRule.Text.Replace("|", string.Empty);
                    if (!DirectoryUtils.IsDirectoryNameCompliant(filePathRule))
                    {
                        FailMessage("内容页面命名规则不符合系统要求！");
                        return;
                    }
                    if (PathUtils.IsDirectoryPath(filePathRule))
                    {
                        FailMessage("内容页面命名规则必须包含生成文件的后缀！");
                        return;
                    }
                }

                if (tbFilePath.Text != PageUtility.GetInputChannelUrl(PublishmentSystemInfo, nodeInfo))
                {
                    nodeInfo.FilePath = tbFilePath.Text;
                }
                if (tbChannelFilePathRule.Text != PathUtility.GetChannelFilePathRule(PublishmentSystemInfo, _nodeId))
                {
                    nodeInfo.ChannelFilePathRule = tbChannelFilePathRule.Text;
                }
                if (tbContentFilePathRule.Text != PathUtility.GetContentFilePathRule(PublishmentSystemInfo, _nodeId))
                {
                    nodeInfo.ContentFilePathRule = tbContentFilePathRule.Text;
                }

                DataProvider.NodeDao.UpdateNodeInfo(nodeInfo);

                CreateManager.CreateChannel(PublishmentSystemId, _nodeId);

                Body.AddSiteLog(PublishmentSystemId, _nodeId, 0, "设置页面命名规则", $"栏目:{nodeInfo.NodeName}");

                isSuccess = true;
            }
            catch (Exception ex)
            {
                FailMessage(ex, ex.Message);
            }

            if (isSuccess)
            {
                PageUtils.CloseModalPageAndRedirect(Page, PageTemplateFilePathRule.GetRedirectUrl(PublishmentSystemId, _nodeId));
            }
        }
	}
}

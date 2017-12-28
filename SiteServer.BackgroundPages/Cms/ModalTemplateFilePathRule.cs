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
        public PlaceHolder PhFilePath;
        public TextBox TbFilePath;
        public TextBox TbChannelFilePathRule;
        public TextBox TbContentFilePathRule;
        public Button BtnCreateChannelRule;
        public Button BtnCreateContentRule;

		private int _nodeId;

        public static string GetOpenWindowString(int publishmentSystemId, int nodeId)
        {
            return LayerUtils.GetOpenScript("页面命名规则",
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

            if (IsPostBack) return;

            var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, _nodeId);
            var linkType = ELinkTypeUtils.GetEnumType(nodeInfo.LinkType);
            if (nodeInfo.ParentId == 0 || linkType == ELinkType.LinkToFirstChannel || linkType == ELinkType.LinkToFirstContent || linkType == ELinkType.LinkToLastAddChannel || linkType == ELinkType.NoLink)
            {
                PhFilePath.Visible = false;
            }

            var showPopWinString = ModalFilePathRule.GetOpenWindowString(PublishmentSystemId, _nodeId, true, TbChannelFilePathRule.ClientID);
            BtnCreateChannelRule.Attributes.Add("onclick", showPopWinString);

            showPopWinString = ModalFilePathRule.GetOpenWindowString(PublishmentSystemId, _nodeId, false, TbContentFilePathRule.ClientID);
            BtnCreateContentRule.Attributes.Add("onclick", showPopWinString);

            TbFilePath.Text = string.IsNullOrEmpty(nodeInfo.FilePath) ? PageUtility.GetInputChannelUrl(PublishmentSystemInfo, nodeInfo, false) : nodeInfo.FilePath;

            TbChannelFilePathRule.Text = string.IsNullOrEmpty(nodeInfo.ChannelFilePathRule) ? PathUtility.GetChannelFilePathRule(PublishmentSystemInfo, _nodeId) : nodeInfo.ChannelFilePathRule;

            TbContentFilePathRule.Text = string.IsNullOrEmpty(nodeInfo.ContentFilePathRule) ? PathUtility.GetContentFilePathRule(PublishmentSystemInfo, _nodeId) : nodeInfo.ContentFilePathRule;
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isSuccess = false;

            try
            {
                var nodeInfo = NodeManager.GetNodeInfo(PublishmentSystemId, _nodeId);

                var filePath = nodeInfo.FilePath;

                if (PhFilePath.Visible)
                {
                    TbFilePath.Text = TbFilePath.Text.Trim();
                    if (!string.IsNullOrEmpty(TbFilePath.Text) && !StringUtils.EqualsIgnoreCase(filePath, TbFilePath.Text))
                    {
                        if (!DirectoryUtils.IsDirectoryNameCompliant(TbFilePath.Text))
                        {
                            FailMessage("栏目页面路径不符合系统要求！");
                            return;
                        }

                        if (PathUtils.IsDirectoryPath(TbFilePath.Text))
                        {
                            TbFilePath.Text = PageUtils.Combine(TbFilePath.Text, "index.html");
                        }

                        var filePathArrayList = DataProvider.NodeDao.GetAllFilePathByPublishmentSystemId(PublishmentSystemId);
                        filePathArrayList.AddRange(DataProvider.TemplateMatchDao.GetAllFilePathByPublishmentSystemId(PublishmentSystemId));
                        if (filePathArrayList.IndexOf(TbFilePath.Text) != -1)
                        {
                            FailMessage("栏目修改失败，栏目页面路径已存在！");
                            return;
                        }
                    }
                }

                if (!string.IsNullOrEmpty(TbChannelFilePathRule.Text))
                {
                    var filePathRule = TbChannelFilePathRule.Text.Replace("|", string.Empty);
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

                if (!string.IsNullOrEmpty(TbContentFilePathRule.Text))
                {
                    var filePathRule = TbContentFilePathRule.Text.Replace("|", string.Empty);
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

                if (TbFilePath.Text != PageUtility.GetInputChannelUrl(PublishmentSystemInfo, nodeInfo, false))
                {
                    nodeInfo.FilePath = TbFilePath.Text;
                }
                if (TbChannelFilePathRule.Text != PathUtility.GetChannelFilePathRule(PublishmentSystemInfo, _nodeId))
                {
                    nodeInfo.ChannelFilePathRule = TbChannelFilePathRule.Text;
                }
                if (TbContentFilePathRule.Text != PathUtility.GetContentFilePathRule(PublishmentSystemInfo, _nodeId))
                {
                    nodeInfo.ContentFilePathRule = TbContentFilePathRule.Text;
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
                LayerUtils.CloseAndRedirect(Page, PageTemplateFilePathRule.GetRedirectUrl(PublishmentSystemId, _nodeId));
            }
        }
	}
}

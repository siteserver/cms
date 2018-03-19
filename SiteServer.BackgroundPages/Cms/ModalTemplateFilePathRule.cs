using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
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

		private int _channelId;

        public static string GetOpenWindowString(int siteId, int channelId)
        {
            return LayerUtils.GetOpenScript("页面命名规则",
                PageUtils.GetCmsUrl(siteId, nameof(ModalTemplateFilePathRule), new NameValueCollection
                {
                    {"channelId", channelId.ToString()}
                }));
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId", "channelId");
            _channelId = AuthRequest.GetQueryInt("channelId");

            if (IsPostBack) return;

            var channelInfo = ChannelManager.GetChannelInfo(SiteId, _channelId);
            var linkType = ELinkTypeUtils.GetEnumType(channelInfo.LinkType);
            if (channelInfo.ParentId == 0 || linkType == ELinkType.LinkToFirstChannel || linkType == ELinkType.LinkToFirstContent || linkType == ELinkType.LinkToLastAddChannel || linkType == ELinkType.NoLink)
            {
                PhFilePath.Visible = false;
            }

            var showPopWinString = ModalFilePathRule.GetOpenWindowString(SiteId, _channelId, true, TbChannelFilePathRule.ClientID);
            BtnCreateChannelRule.Attributes.Add("onclick", showPopWinString);

            showPopWinString = ModalFilePathRule.GetOpenWindowString(SiteId, _channelId, false, TbContentFilePathRule.ClientID);
            BtnCreateContentRule.Attributes.Add("onclick", showPopWinString);

            TbFilePath.Text = string.IsNullOrEmpty(channelInfo.FilePath) ? PageUtility.GetInputChannelUrl(SiteInfo, channelInfo, false) : channelInfo.FilePath;

            TbChannelFilePathRule.Text = string.IsNullOrEmpty(channelInfo.ChannelFilePathRule) ? PathUtility.GetChannelFilePathRule(SiteInfo, _channelId) : channelInfo.ChannelFilePathRule;

            TbContentFilePathRule.Text = string.IsNullOrEmpty(channelInfo.ContentFilePathRule) ? PathUtility.GetContentFilePathRule(SiteInfo, _channelId) : channelInfo.ContentFilePathRule;
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isSuccess = false;

            try
            {
                var channelInfo = ChannelManager.GetChannelInfo(SiteId, _channelId);

                var filePath = channelInfo.FilePath;

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

                        var filePathArrayList = DataProvider.ChannelDao.GetAllFilePathBySiteId(SiteId);
                        filePathArrayList.AddRange(DataProvider.TemplateMatchDao.GetAllFilePathBySiteId(SiteId));
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

                if (TbFilePath.Text != PageUtility.GetInputChannelUrl(SiteInfo, channelInfo, false))
                {
                    channelInfo.FilePath = TbFilePath.Text;
                }
                if (TbChannelFilePathRule.Text != PathUtility.GetChannelFilePathRule(SiteInfo, _channelId))
                {
                    channelInfo.ChannelFilePathRule = TbChannelFilePathRule.Text;
                }
                if (TbContentFilePathRule.Text != PathUtility.GetContentFilePathRule(SiteInfo, _channelId))
                {
                    channelInfo.ContentFilePathRule = TbContentFilePathRule.Text;
                }

                DataProvider.ChannelDao.Update(channelInfo);

                CreateManager.CreateChannel(SiteId, _channelId);

                AuthRequest.AddSiteLog(SiteId, _channelId, 0, "设置页面命名规则", $"栏目:{channelInfo.ChannelName}");

                isSuccess = true;
            }
            catch (Exception ex)
            {
                FailMessage(ex, ex.Message);
            }

            if (isSuccess)
            {
                LayerUtils.CloseAndRedirect(Page, PageConfigurationCreateRule.GetRedirectUrl(SiteId, _channelId));
            }
        }
	}
}

using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web.UI.WebControls;
using SiteServer.CMS.Context;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Enumerations;

namespace SiteServer.BackgroundPages.Cms
{
    public class ModalTemplateFilePathRule : BasePageCms
    {
        public PlaceHolder PhChannel;
        public TextBox TbLinkUrl;
        public DropDownList DdlLinkType;
        public TextBox TbFilePath;
        
        public TextBox TbChannelFilePathRule;
        public TextBox TbContentFilePathRule;
        public Button BtnCreateChannelRule;
        public Button BtnCreateContentRule;

        public RadioButtonList RblIsChannelCreatable;
        public RadioButtonList RblIsContentCreatable;

        private int _channelId;

        public static string GetOpenWindowString(int siteId, int channelId)
        {
            return LayerUtils.GetOpenScript("页面生成规则",
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

            var channelInfo = ChannelManager.GetChannelAsync(SiteId, _channelId).GetAwaiter().GetResult();
            if (SiteId == _channelId)
            {
                PhChannel.Visible = false;
            }
            else
            {
                TbLinkUrl.Text = channelInfo.LinkUrl;

                ELinkTypeUtils.AddListItems(DdlLinkType);
                ControlUtils.SelectSingleItem(DdlLinkType, channelInfo.LinkType);

                TbFilePath.Text = string.IsNullOrEmpty(channelInfo.FilePath) ? PageUtility.GetInputChannelUrlAsync(Site, channelInfo, false).GetAwaiter().GetResult() : channelInfo.FilePath;
            }

            TbChannelFilePathRule.Text = string.IsNullOrEmpty(channelInfo.ChannelFilePathRule) ? PathUtility.GetChannelFilePathRuleAsync(Site, _channelId).GetAwaiter().GetResult() : channelInfo.ChannelFilePathRule;
            BtnCreateChannelRule.Attributes.Add("onclick", ModalFilePathRule.GetOpenWindowString(SiteId, _channelId, true, TbChannelFilePathRule.ClientID));

            TbContentFilePathRule.Text = string.IsNullOrEmpty(channelInfo.ContentFilePathRule) ? PathUtility.GetContentFilePathRuleAsync(Site, _channelId).GetAwaiter().GetResult() : channelInfo.ContentFilePathRule;
            BtnCreateContentRule.Attributes.Add("onclick", ModalFilePathRule.GetOpenWindowString(SiteId, _channelId, false, TbContentFilePathRule.ClientID));

            ControlUtils.SelectSingleItem(RblIsChannelCreatable, channelInfo.IsChannelCreatable.ToString());
            ControlUtils.SelectSingleItem(RblIsContentCreatable, channelInfo.IsContentCreatable.ToString());
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            var isSuccess = false;

            try
            {
                var channelInfo = ChannelManager.GetChannelAsync(SiteId, _channelId).GetAwaiter().GetResult();

                if (PhChannel.Visible)
                {
                    channelInfo.LinkUrl = TbLinkUrl.Text;
                    channelInfo.LinkType = DdlLinkType.SelectedValue;

                    var filePath = channelInfo.FilePath;
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

                        var filePathArrayList = DataProvider.ChannelDao.GetAllFilePathBySiteIdAsync(SiteId).GetAwaiter().GetResult();
                        if (filePathArrayList.Contains(TbFilePath.Text))
                        {
                            FailMessage("栏目修改失败，栏目页面路径已存在！");
                            return;
                        }
                    }

                    if (TbFilePath.Text != PageUtility.GetInputChannelUrlAsync(Site, channelInfo, false).GetAwaiter().GetResult())
                    {
                        channelInfo.FilePath = TbFilePath.Text;
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
                
                if (TbChannelFilePathRule.Text != PathUtility.GetChannelFilePathRuleAsync(Site, _channelId).GetAwaiter().GetResult())
                {
                    channelInfo.ChannelFilePathRule = TbChannelFilePathRule.Text;
                }
                if (TbContentFilePathRule.Text != PathUtility.GetContentFilePathRuleAsync(Site, _channelId).GetAwaiter().GetResult())
                {
                    channelInfo.ContentFilePathRule = TbContentFilePathRule.Text;
                }

                channelInfo.IsChannelCreatable = TranslateUtils.ToBool(RblIsChannelCreatable.SelectedValue);
                channelInfo.IsContentCreatable = TranslateUtils.ToBool(RblIsContentCreatable.SelectedValue);

                DataProvider.ChannelDao.UpdateAsync(channelInfo).GetAwaiter().GetResult();

                CreateManager.CreateChannelAsync(SiteId, _channelId).GetAwaiter().GetResult();

                AuthRequest.AddSiteLogAsync(SiteId, _channelId, 0, "设置页面生成规则", $"栏目:{channelInfo.ChannelName}").GetAwaiter().GetResult();

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

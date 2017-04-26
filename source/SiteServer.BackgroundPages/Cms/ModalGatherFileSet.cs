using System;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Cms
{
	public class ModalGatherFileSet : BasePageCms
    {		
        public TextBox GatherUrl;

        public PlaceHolder PlaceHolder_File;
        public TextBox FilePath;
        public RadioButtonList IsSaveRelatedFiles;
        public RadioButtonList IsRemoveScripts;
        public PlaceHolder PlaceHolder_File_Directory;
        public TextBox StyleDirectoryPath;
        public TextBox ScriptDirectoryPath;
        public TextBox ImageDirectoryPath;

        public PlaceHolder PlaceHolder_Content;
		protected DropDownList NodeIDDropDownList;
        public RadioButtonList IsSaveImage;

		private string _gatherRuleName;

        public static string GetOpenWindowString(int publishmentSystemId, string gatherRuleName)
        {
            return PageUtils.GetOpenWindowString("信息采集", PageUtils.GetCmsUrl(nameof(ModalGatherFileSet), new NameValueCollection
            {
                {"PublishmentSystemID", publishmentSystemId.ToString()},
                {"GatherRuleName", gatherRuleName}
            }));
        }

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID", "GatherRuleName");

            _gatherRuleName = Body.GetQueryString("GatherRuleName");

			if (!IsPostBack)
			{
                InfoMessage("采集名称：" + _gatherRuleName);

                var gatherFileRuleInfo = DataProvider.GatherFileRuleDao.GetGatherFileRuleInfo(_gatherRuleName, PublishmentSystemId);
                GatherUrl.Text = gatherFileRuleInfo.GatherUrl;
                if (gatherFileRuleInfo.IsToFile)
                {
                    PlaceHolder_File.Visible = true;
                    PlaceHolder_Content.Visible = false;

                    FilePath.Text = gatherFileRuleInfo.FilePath;
                    ControlUtils.SelectListItems(IsSaveRelatedFiles, gatherFileRuleInfo.IsSaveRelatedFiles.ToString());
                    ControlUtils.SelectListItems(IsRemoveScripts, gatherFileRuleInfo.IsRemoveScripts.ToString());
                    StyleDirectoryPath.Text = gatherFileRuleInfo.StyleDirectoryPath;
                    ScriptDirectoryPath.Text = gatherFileRuleInfo.ScriptDirectoryPath;
                    ImageDirectoryPath.Text = gatherFileRuleInfo.ImageDirectoryPath;
                }
                else
                {
                    PlaceHolder_File.Visible = false;
                    PlaceHolder_Content.Visible = true;

                    NodeManager.AddListItemsForAddContent(NodeIDDropDownList.Items, PublishmentSystemInfo, true, Body.AdministratorName);
                    ControlUtils.SelectListItems(NodeIDDropDownList, gatherFileRuleInfo.NodeId.ToString());
                    ControlUtils.SelectListItems(IsSaveImage, gatherFileRuleInfo.IsSaveImage.ToString());
                }

				
			}
		}

        public void DropDownList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (PlaceHolder_File.Visible)
            {
                PlaceHolder_File_Directory.Visible = TranslateUtils.ToBool(IsSaveRelatedFiles.SelectedValue);
            }
        }

        public override void Submit_OnClick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(GatherUrl.Text))
            {
                FailMessage("必须填写采集网页地址！");
                return;
            }

            if (PlaceHolder_File.Visible)
            {
                if (string.IsNullOrEmpty(FilePath.Text))
                {
                    FailMessage("必须填写采集到的文件地址！");
                    return;
                }
                else
                {
                    var isOk = false;
                    if (StringUtils.StringStartsWith(FilePath.Text, '~') || StringUtils.StringStartsWith(FilePath.Text, '@'))
                    {
                        if (!PathUtils.IsDirectoryPath(FilePath.Text))
                        {
                            isOk = true;
                        }
                    }
                    if (isOk == false)
                    {
                        FailMessage("采集到的文件地址不正确,必须填写有效的文件地址！");
                        return;
                    }
                }

                if (TranslateUtils.ToBool(IsSaveRelatedFiles.SelectedValue))
                {
                    var isOk = false;
                    if (StringUtils.StringStartsWith(StyleDirectoryPath.Text, '~') || StringUtils.StringStartsWith(StyleDirectoryPath.Text, '@'))
                    {
                        if (PathUtils.IsDirectoryPath(StyleDirectoryPath.Text))
                        {
                            isOk = true;
                        }
                    }
                    if (isOk == false)
                    {
                        FailMessage("CSS样式保存地址不正确,必须填写有效的文件夹地址！");
                        return;
                    }
                    isOk = false;
                    if (StringUtils.StringStartsWith(ScriptDirectoryPath.Text, '~') || StringUtils.StringStartsWith(ScriptDirectoryPath.Text, '@'))
                    {
                        if (PathUtils.IsDirectoryPath(ScriptDirectoryPath.Text))
                        {
                            isOk = true;
                        }
                    }
                    if (isOk == false)
                    {
                        FailMessage("Js脚本保存地址不正确,必须填写有效的文件夹地址！");
                        return;
                    }
                    isOk = false;
                    if (StringUtils.StringStartsWith(ImageDirectoryPath.Text, '~') || StringUtils.StringStartsWith(ImageDirectoryPath.Text, '@'))
                    {
                        if (PathUtils.IsDirectoryPath(ImageDirectoryPath.Text))
                        {
                            isOk = true;
                        }
                    }
                    if (isOk == false)
                    {
                        FailMessage("图片保存地址不正确,必须填写有效的文件夹地址！");
                        return;
                    }
                }
            }

            var gatherFileRuleInfo = DataProvider.GatherFileRuleDao.GetGatherFileRuleInfo(_gatherRuleName, PublishmentSystemId);

            gatherFileRuleInfo.GatherUrl = GatherUrl.Text;
            gatherFileRuleInfo.FilePath = FilePath.Text;
            gatherFileRuleInfo.IsSaveRelatedFiles = TranslateUtils.ToBool(IsSaveRelatedFiles.SelectedValue);
            gatherFileRuleInfo.IsRemoveScripts = TranslateUtils.ToBool(IsRemoveScripts.SelectedValue);
            gatherFileRuleInfo.StyleDirectoryPath = StyleDirectoryPath.Text;
            gatherFileRuleInfo.ScriptDirectoryPath = ScriptDirectoryPath.Text;
            gatherFileRuleInfo.ImageDirectoryPath = ImageDirectoryPath.Text;
            gatherFileRuleInfo.NodeId = TranslateUtils.ToInt(NodeIDDropDownList.SelectedValue);
            gatherFileRuleInfo.IsSaveImage = TranslateUtils.ToBool(IsSaveImage.SelectedValue);

            DataProvider.GatherFileRuleDao.Update(gatherFileRuleInfo);

            PageUtils.Redirect(ModalProgressBar.GetRedirectUrlStringWithGatherFile(PublishmentSystemId, _gatherRuleName));
		}
	}
}

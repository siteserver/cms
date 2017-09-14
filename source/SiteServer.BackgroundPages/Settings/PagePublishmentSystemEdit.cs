using System;
using System.Collections;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Settings
{
	public class PagePublishmentSystemEdit : BasePageCms
    {
		public TextBox PublishmentSystemName;
        public DropDownList ParentPublishmentSystemID;
		public DropDownList AuxiliaryTableForContent;
        public TextBox Taxis;
		public RadioButtonList IsCheckContentUseLevel;
		public DropDownList CheckContentLevel;
		public TextBox PublishmentSystemDir;
		public Control PublishmentSystemDirRow;

        public HtmlTableRow ParentPublishmentSystemIDRow;
		public HtmlTableRow CheckContentLevelRow;

        public Button Submit;

		public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

			if (!IsPostBack)
            {
                BreadCrumbSettings("修改站点", AppManager.Permissions.Settings.SiteManagement);

                if (PublishmentSystemInfo.IsHeadquarters)
                {
                    ParentPublishmentSystemIDRow.Visible = false;
                }
                else
                {
                    ParentPublishmentSystemIDRow.Visible = true;

                    ParentPublishmentSystemID.Items.Add(new ListItem("<无上级站点>", "0"));
                    var publishmentSystemIDArrayList = PublishmentSystemManager.GetPublishmentSystemIdList();
                    var mySystemInfoArrayList = new ArrayList();
                    var parentWithChildren = new Hashtable();
                    foreach (int publishmentSystemID in publishmentSystemIDArrayList)
                    {
                        if (publishmentSystemID == PublishmentSystemId) continue;
                        var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemID);
                        if (publishmentSystemInfo.IsHeadquarters == false)
                        {
                            if (publishmentSystemInfo.ParentPublishmentSystemId == 0)
                            {
                                mySystemInfoArrayList.Add(publishmentSystemInfo);
                            }
                            else
                            {
                                var children = new ArrayList();
                                if (parentWithChildren.Contains(publishmentSystemInfo.ParentPublishmentSystemId))
                                {
                                    children = (ArrayList)parentWithChildren[publishmentSystemInfo.ParentPublishmentSystemId];
                                }
                                children.Add(publishmentSystemInfo);
                                parentWithChildren[publishmentSystemInfo.ParentPublishmentSystemId] = children;
                            }
                        }
                    }
                    foreach (PublishmentSystemInfo publishmentSystemInfo in mySystemInfoArrayList)
                    {
                        AddSite(ParentPublishmentSystemID, publishmentSystemInfo, parentWithChildren, 0);
                    }
                    ControlUtils.SelectListItems(ParentPublishmentSystemID, PublishmentSystemInfo.ParentPublishmentSystemId.ToString());
                }

                var tableList = BaiRongDataProvider.TableCollectionDao.GetAuxiliaryTableListCreatedInDbByAuxiliaryTableType(EAuxiliaryTableType.BackgroundContent);
				foreach (AuxiliaryTableInfo tableInfo in tableList)
				{
                    var li = new ListItem($"{tableInfo.TableCnName}({tableInfo.TableEnName})", tableInfo.TableEnName);
					AuxiliaryTableForContent.Items.Add(li);
				}

                Taxis.Text = PublishmentSystemInfo.Taxis.ToString();

				IsCheckContentUseLevel.Items.Add(new ListItem("默认审核机制",false.ToString()));
                IsCheckContentUseLevel.Items.Add(new ListItem("多级审核机制", true.ToString()));

                if (PublishmentSystemInfo == null)
                {
                    PageUtils.RedirectToErrorPage("站点不存在，请确认后再试！");
                    return;
                }
                PublishmentSystemName.Text = PublishmentSystemInfo.PublishmentSystemName;
                ControlUtils.SelectListItems(IsCheckContentUseLevel, PublishmentSystemInfo.IsCheckContentUseLevel.ToString());
                if (PublishmentSystemInfo.IsCheckContentUseLevel)
                {
                    ControlUtils.SelectListItems(CheckContentLevel, PublishmentSystemInfo.CheckContentLevel.ToString());
                    CheckContentLevelRow.Visible = true;
                }
                else
                {
                    CheckContentLevelRow.Visible = false;
                }
                if (!string.IsNullOrEmpty(PublishmentSystemInfo.PublishmentSystemDir))
                {
                    PublishmentSystemDir.Text = PathUtils.GetDirectoryName(PublishmentSystemInfo.PublishmentSystemDir);
                }
                if (PublishmentSystemInfo.IsHeadquarters)
                {
                    PublishmentSystemDirRow.Visible = false;
                }
                foreach (ListItem item in AuxiliaryTableForContent.Items)
                {
                    if (item.Value.Equals(PublishmentSystemInfo.AuxiliaryTableForContent))
                    {
                        item.Selected = true;
                    }
                    else
                    {
                        item.Selected = false;
                    }
                }

                Submit.Attributes.Add("onclick", GetShowHintScript());
			}
		}

        private static void AddSite(ListControl listControl, PublishmentSystemInfo publishmentSystemInfo, Hashtable parentWithChildren, int level)
        {
            var padding = string.Empty;
            for (var i = 0; i < level; i++)
            {
                padding += "　";
            }
            if (level > 0)
            {
                padding += "└ ";
            }

            if (parentWithChildren[publishmentSystemInfo.PublishmentSystemId] != null)
            {
                var children = (ArrayList)parentWithChildren[publishmentSystemInfo.PublishmentSystemId];
                listControl.Items.Add(new ListItem(padding + publishmentSystemInfo.PublishmentSystemName +
                                                   $"({children.Count})", publishmentSystemInfo.PublishmentSystemId.ToString()));
                level++;
                foreach (PublishmentSystemInfo subSiteInfo in children)
                {
                    AddSite(listControl, subSiteInfo, parentWithChildren, level);
                }
            }
            else
            {
                listControl.Items.Add(new ListItem(padding + publishmentSystemInfo.PublishmentSystemName, publishmentSystemInfo.PublishmentSystemId.ToString()));
            }
        }

		public void IsCheckContentUseLevel_OnSelectedIndexChanged(object sender, EventArgs e)
		{
			if (EBooleanUtils.Equals(IsCheckContentUseLevel.SelectedValue, EBoolean.True))
			{
				CheckContentLevelRow.Visible = true;
			}
			else
			{
				CheckContentLevelRow.Visible = false;
			}
		}

		public override void Submit_OnClick(object sender, EventArgs e)
		{
			if (Page.IsPostBack && Page.IsValid)
			{
                PublishmentSystemInfo.PublishmentSystemName = PublishmentSystemName.Text;
                PublishmentSystemInfo.Taxis = TranslateUtils.ToInt(Taxis.Text);
                PublishmentSystemInfo.IsCheckContentUseLevel = TranslateUtils.ToBool(IsCheckContentUseLevel.SelectedValue);
                if (PublishmentSystemInfo.IsCheckContentUseLevel)
                {
                    PublishmentSystemInfo.CheckContentLevel = TranslateUtils.ToInt(CheckContentLevel.SelectedValue);
                }

                var isTableChanged = false;

                if (PublishmentSystemInfo.AuxiliaryTableForContent != AuxiliaryTableForContent.SelectedValue)
                {
                    isTableChanged = true;
                    PublishmentSystemInfo.AuxiliaryTableForContent = AuxiliaryTableForContent.SelectedValue;
                }

                if (PublishmentSystemInfo.IsHeadquarters == false)
                {
                    if (!StringUtils.EqualsIgnoreCase(PathUtils.GetDirectoryName(PublishmentSystemInfo.PublishmentSystemDir), PublishmentSystemDir.Text))
                    {
                        var list = DataProvider.NodeDao.GetLowerSystemDirList(PublishmentSystemInfo.ParentPublishmentSystemId);
                        if (list.IndexOf(PublishmentSystemDir.Text.ToLower()) != -1)
                        {
                            FailMessage("站点修改失败，已存在相同的发布路径！");
                            return;
                        }

                        try
                        {
                            var parentPSPath = WebConfigUtils.PhysicalApplicationPath;
                            if (PublishmentSystemInfo.ParentPublishmentSystemId > 0)
                            {
                                var parentPublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(PublishmentSystemInfo.ParentPublishmentSystemId);
                                parentPSPath = PathUtility.GetPublishmentSystemPath(parentPublishmentSystemInfo);
                            }
                            DirectoryUtility.ChangePublishmentSystemDir(parentPSPath, PublishmentSystemInfo.PublishmentSystemDir, PublishmentSystemDir.Text);
                        }
                        catch (Exception ex)
                        {
                            FailMessage(ex, "站点修改失败，发布路径文件夹已存在！");
                            return;
                        }
                    }

                    if (ParentPublishmentSystemIDRow.Visible && PublishmentSystemInfo.ParentPublishmentSystemId != TranslateUtils.ToInt(ParentPublishmentSystemID.SelectedValue))
                    {
                        var newParentPublishmentSystemID = TranslateUtils.ToInt(ParentPublishmentSystemID.SelectedValue);
                        var list = DataProvider.NodeDao.GetLowerSystemDirList(newParentPublishmentSystemID);
                        if (list.IndexOf(PublishmentSystemDir.Text.ToLower()) != -1)
                        {
                            FailMessage("站点修改失败，已存在相同的发布路径！");
                            return;
                        }

                        try
                        {
                            DirectoryUtility.ChangeParentPublishmentSystem(PublishmentSystemInfo.ParentPublishmentSystemId, TranslateUtils.ToInt(ParentPublishmentSystemID.SelectedValue), PublishmentSystemId, PublishmentSystemDir.Text);
                            PublishmentSystemInfo.ParentPublishmentSystemId = newParentPublishmentSystemID;
                        }
                        catch (Exception ex)
                        {
                            FailMessage(ex, "站点修改失败，发布路径文件夹已存在！");   
                            return;
                        }
                    }

                    PublishmentSystemInfo.PublishmentSystemDir = PublishmentSystemDir.Text;
                }

                try
                {
                    DataProvider.PublishmentSystemDao.Update(PublishmentSystemInfo);
                    if (isTableChanged)
                    {
                        DataProvider.NodeDao.UpdateContentNum(PublishmentSystemInfo);
                    }

                    Body.AddAdminLog("修改站点属性", $"站点:{PublishmentSystemInfo.PublishmentSystemName}");

                    SuccessMessage("站点修改成功！");
                    AddWaitAndRedirectScript(PagePublishmentSystem.GetRedirectUrl());
                }
                catch (Exception ex)
                {
                    FailMessage(ex, "站点修改失败！");
                }
			}
		}

	}
}

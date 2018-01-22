using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.Utils.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.BackgroundPages.Settings
{
	public class PageSiteEdit : BasePageCms
    {
		public TextBox TbPublishmentSystemName;
        public PlaceHolder PhPublishmentSystemDir;
        public TextBox TbPublishmentSystemDir;
        public PlaceHolder PhParentPublishmentSystemId;
        public DropDownList DdlParentPublishmentSystemId;
		public DropDownList DdlAuxiliaryTableForContent;
        public TextBox TbTaxis;
		public RadioButtonList RblIsCheckContentUseLevel;
        public PlaceHolder PhCheckContentLevel;
        public DropDownList DdlCheckContentLevel;
        public Button BtnSubmit;

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetSettingsUrl(nameof(PageSiteEdit), new NameValueCollection
            {
                {"publishmentSystemId", publishmentSystemId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            if (IsPostBack) return;

            VerifyAdministratorPermissions(AppManager.Permissions.Settings.Site);

            if (PublishmentSystemInfo.IsHeadquarters)
            {
                PhParentPublishmentSystemId.Visible = false;
            }
            else
            {
                PhParentPublishmentSystemId.Visible = true;

                DdlParentPublishmentSystemId.Items.Add(new ListItem("<无上级站点>", "0"));
                var publishmentSystemIdList = PublishmentSystemManager.GetPublishmentSystemIdList();
                var mySystemInfoArrayList = new ArrayList();
                var parentWithChildren = new Hashtable();
                foreach (var publishmentSystemId in publishmentSystemIdList)
                {
                    if (publishmentSystemId == PublishmentSystemId) continue;
                    var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
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
                    AddSite(DdlParentPublishmentSystemId, publishmentSystemInfo, parentWithChildren, 0);
                }
                ControlUtils.SelectSingleItem(DdlParentPublishmentSystemId, PublishmentSystemInfo.ParentPublishmentSystemId.ToString());
            }

            var tableList = DataProvider.TableCollectionDao.GetTableCollectionInfoListCreatedInDb();
            foreach (var tableInfo in tableList)
            {
                var li = new ListItem($"{tableInfo.TableCnName}({tableInfo.TableEnName})", tableInfo.TableEnName);
                DdlAuxiliaryTableForContent.Items.Add(li);
            }

            TbTaxis.Text = PublishmentSystemInfo.Taxis.ToString();

            RblIsCheckContentUseLevel.Items.Add(new ListItem("默认审核机制",false.ToString()));
            RblIsCheckContentUseLevel.Items.Add(new ListItem("多级审核机制", true.ToString()));

            if (PublishmentSystemInfo == null)
            {
                PageUtils.RedirectToErrorPage("站点不存在，请确认后再试！");
                return;
            }
            TbPublishmentSystemName.Text = PublishmentSystemInfo.PublishmentSystemName;
            ControlUtils.SelectSingleItem(RblIsCheckContentUseLevel, PublishmentSystemInfo.IsCheckContentUseLevel.ToString());
            if (PublishmentSystemInfo.IsCheckContentUseLevel)
            {
                ControlUtils.SelectSingleItem(DdlCheckContentLevel, PublishmentSystemInfo.CheckContentLevel.ToString());
                PhCheckContentLevel.Visible = true;
            }
            else
            {
                PhCheckContentLevel.Visible = false;
            }
            if (!string.IsNullOrEmpty(PublishmentSystemInfo.PublishmentSystemDir))
            {
                TbPublishmentSystemDir.Text = PathUtils.GetDirectoryName(PublishmentSystemInfo.PublishmentSystemDir);
            }
            if (PublishmentSystemInfo.IsHeadquarters)
            {
                PhPublishmentSystemDir.Visible = false;
            }
            foreach (ListItem item in DdlAuxiliaryTableForContent.Items)
            {
                item.Selected = item.Value.Equals(PublishmentSystemInfo.AuxiliaryTableForContent);
            }

            BtnSubmit.Attributes.Add("onclick", GetShowHintScript());
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

		public void RblIsCheckContentUseLevel_OnSelectedIndexChanged(object sender, EventArgs e)
		{
		    PhCheckContentLevel.Visible = EBooleanUtils.Equals(RblIsCheckContentUseLevel.SelectedValue, EBoolean.True);
		}

		public override void Submit_OnClick(object sender, EventArgs e)
		{
		    if (!Page.IsPostBack || !Page.IsValid) return;

		    PublishmentSystemInfo.PublishmentSystemName = TbPublishmentSystemName.Text;
		    PublishmentSystemInfo.Taxis = TranslateUtils.ToInt(TbTaxis.Text);
		    PublishmentSystemInfo.IsCheckContentUseLevel = TranslateUtils.ToBool(RblIsCheckContentUseLevel.SelectedValue);
		    if (PublishmentSystemInfo.IsCheckContentUseLevel)
		    {
		        PublishmentSystemInfo.CheckContentLevel = TranslateUtils.ToInt(DdlCheckContentLevel.SelectedValue);
		    }

		    var isTableChanged = false;

		    if (PublishmentSystemInfo.AuxiliaryTableForContent != DdlAuxiliaryTableForContent.SelectedValue)
		    {
		        isTableChanged = true;
		        PublishmentSystemInfo.AuxiliaryTableForContent = DdlAuxiliaryTableForContent.SelectedValue;
		    }

		    if (PublishmentSystemInfo.IsHeadquarters == false)
		    {
		        if (!StringUtils.EqualsIgnoreCase(PathUtils.GetDirectoryName(PublishmentSystemInfo.PublishmentSystemDir), TbPublishmentSystemDir.Text))
		        {
		            var list = DataProvider.NodeDao.GetLowerSystemDirList(PublishmentSystemInfo.ParentPublishmentSystemId);
		            if (list.IndexOf(TbPublishmentSystemDir.Text.ToLower()) != -1)
		            {
		                FailMessage("站点修改失败，已存在相同的发布路径！");
		                return;
		            }

                    var parentPsPath = WebConfigUtils.PhysicalApplicationPath;
                    if (PublishmentSystemInfo.ParentPublishmentSystemId > 0)
                    {
                        var parentPublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(PublishmentSystemInfo.ParentPublishmentSystemId);
                        parentPsPath = PathUtility.GetPublishmentSystemPath(parentPublishmentSystemInfo);
                    }
                    DirectoryUtility.ChangePublishmentSystemDir(parentPsPath, PublishmentSystemInfo.PublishmentSystemDir, TbPublishmentSystemDir.Text);
                }

		        if (PhParentPublishmentSystemId.Visible && PublishmentSystemInfo.ParentPublishmentSystemId != TranslateUtils.ToInt(DdlParentPublishmentSystemId.SelectedValue))
		        {
		            var newParentPublishmentSystemId = TranslateUtils.ToInt(DdlParentPublishmentSystemId.SelectedValue);
		            var list = DataProvider.NodeDao.GetLowerSystemDirList(newParentPublishmentSystemId);
		            if (list.IndexOf(TbPublishmentSystemDir.Text.ToLower()) != -1)
		            {
		                FailMessage("站点修改失败，已存在相同的发布路径！");
		                return;
		            }

                    DirectoryUtility.ChangeParentPublishmentSystem(PublishmentSystemInfo.ParentPublishmentSystemId, TranslateUtils.ToInt(DdlParentPublishmentSystemId.SelectedValue), PublishmentSystemId, TbPublishmentSystemDir.Text);
                    PublishmentSystemInfo.ParentPublishmentSystemId = newParentPublishmentSystemId;
                }

		        PublishmentSystemInfo.PublishmentSystemDir = TbPublishmentSystemDir.Text;
		    }

            DataProvider.PublishmentSystemDao.Update(PublishmentSystemInfo);
            if (isTableChanged)
            {
                DataProvider.NodeDao.UpdateContentNum(PublishmentSystemInfo);
            }

            Body.AddAdminLog("修改站点属性", $"站点:{PublishmentSystemInfo.PublishmentSystemName}");

            SuccessMessage("站点修改成功！");
            AddWaitAndRedirectScript(PageSite.GetRedirectUrl());
        }

        public void Return_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageSite.GetRedirectUrl());
        }
    }
}

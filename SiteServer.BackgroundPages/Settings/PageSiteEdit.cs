using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.Utils.Enumerations;

namespace SiteServer.BackgroundPages.Settings
{
	public class PageSiteEdit : BasePageCms
    {
		public TextBox TbSiteName;
        public PlaceHolder PhSiteDir;
        public TextBox TbSiteDir;
        public PlaceHolder PhParentId;
        public DropDownList DdlParentId;
		public DropDownList DdlTableName;
        public TextBox TbTaxis;
		public RadioButtonList RblIsCheckContentUseLevel;
        public PlaceHolder PhCheckContentLevel;
        public DropDownList DdlCheckContentLevel;
        public Button BtnSubmit;

        public static string GetRedirectUrl(int siteId)
        {
            return PageUtils.GetSettingsUrl(nameof(PageSiteEdit), new NameValueCollection
            {
                {"siteId", siteId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");

            if (IsPostBack) return;

            VerifySystemPermissions(ConfigManager.SettingsPermissions.Site);

            if (SiteInfo.IsRoot)
            {
                PhParentId.Visible = false;
            }
            else
            {
                PhParentId.Visible = true;

                DdlParentId.Items.Add(new ListItem("<无上级站点>", "0"));
                var siteIdList = SiteManager.GetSiteIdList();
                var mySystemInfoArrayList = new ArrayList();
                var parentWithChildren = new Hashtable();
                foreach (var siteId in siteIdList)
                {
                    if (siteId == SiteId) continue;
                    var siteInfo = SiteManager.GetSiteInfo(siteId);
                    if (siteInfo.IsRoot == false)
                    {
                        if (siteInfo.ParentId == 0)
                        {
                            mySystemInfoArrayList.Add(siteInfo);
                        }
                        else
                        {
                            var children = new ArrayList();
                            if (parentWithChildren.Contains(siteInfo.ParentId))
                            {
                                children = (ArrayList)parentWithChildren[siteInfo.ParentId];
                            }
                            children.Add(siteInfo);
                            parentWithChildren[siteInfo.ParentId] = children;
                        }
                    }
                }
                foreach (SiteInfo siteInfo in mySystemInfoArrayList)
                {
                    AddSite(DdlParentId, siteInfo, parentWithChildren, 0);
                }
                ControlUtils.SelectSingleItem(DdlParentId, SiteInfo.ParentId.ToString());
            }

            var tableList = DataProvider.TableDao.GetTableCollectionInfoListCreatedInDb();
            foreach (var tableInfo in tableList)
            {
                if (tableInfo.DisplayName.StartsWith("插件内容表：")) continue;
                
                var li = new ListItem($"{tableInfo.DisplayName}({tableInfo.TableName})", tableInfo.TableName);
                DdlTableName.Items.Add(li);
            }

            TbTaxis.Text = SiteInfo.Taxis.ToString();

            RblIsCheckContentUseLevel.Items.Add(new ListItem("默认审核机制",false.ToString()));
            RblIsCheckContentUseLevel.Items.Add(new ListItem("多级审核机制", true.ToString()));

            if (SiteInfo == null)
            {
                PageUtils.RedirectToErrorPage("站点不存在，请确认后再试！");
                return;
            }
            TbSiteName.Text = SiteInfo.SiteName;
            ControlUtils.SelectSingleItem(RblIsCheckContentUseLevel, SiteInfo.Additional.IsCheckContentLevel.ToString());
            if (SiteInfo.Additional.IsCheckContentLevel)
            {
                ControlUtils.SelectSingleItem(DdlCheckContentLevel, SiteInfo.Additional.CheckContentLevel.ToString());
                PhCheckContentLevel.Visible = true;
            }
            else
            {
                PhCheckContentLevel.Visible = false;
            }
            if (!string.IsNullOrEmpty(SiteInfo.SiteDir))
            {
                TbSiteDir.Text = PathUtils.GetDirectoryName(SiteInfo.SiteDir, false);
            }
            if (SiteInfo.IsRoot)
            {
                PhSiteDir.Visible = false;
            }
            foreach (ListItem item in DdlTableName.Items)
            {
                item.Selected = item.Value.Equals(SiteInfo.TableName);
            }

            BtnSubmit.Attributes.Add("onclick", GetShowHintScript());
        }

        private static void AddSite(ListControl listControl, SiteInfo siteInfo, Hashtable parentWithChildren, int level)
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

            if (parentWithChildren[siteInfo.Id] != null)
            {
                var children = (ArrayList)parentWithChildren[siteInfo.Id];
                listControl.Items.Add(new ListItem(padding + siteInfo.SiteName +
                                                   $"({children.Count})", siteInfo.Id.ToString()));
                level++;
                foreach (SiteInfo subSiteInfo in children)
                {
                    AddSite(listControl, subSiteInfo, parentWithChildren, level);
                }
            }
            else
            {
                listControl.Items.Add(new ListItem(padding + siteInfo.SiteName, siteInfo.Id.ToString()));
            }
        }

		public void RblIsCheckContentUseLevel_OnSelectedIndexChanged(object sender, EventArgs e)
		{
		    PhCheckContentLevel.Visible = EBooleanUtils.Equals(RblIsCheckContentUseLevel.SelectedValue, EBoolean.True);
		}

		public override void Submit_OnClick(object sender, EventArgs e)
		{
		    if (!Page.IsPostBack || !Page.IsValid) return;

		    SiteInfo.SiteName = TbSiteName.Text;
		    SiteInfo.Taxis = TranslateUtils.ToInt(TbTaxis.Text);
		    SiteInfo.Additional.IsCheckContentLevel = TranslateUtils.ToBool(RblIsCheckContentUseLevel.SelectedValue);
		    if (SiteInfo.Additional.IsCheckContentLevel)
		    {
		        SiteInfo.Additional.CheckContentLevel = TranslateUtils.ToInt(DdlCheckContentLevel.SelectedValue);
		    }

		    var isTableChanged = false;

		    if (SiteInfo.TableName != DdlTableName.SelectedValue)
		    {
		        isTableChanged = true;
		        SiteInfo.TableName = DdlTableName.SelectedValue;
		    }

		    if (SiteInfo.IsRoot == false)
		    {
		        if (!StringUtils.EqualsIgnoreCase(PathUtils.GetDirectoryName(SiteInfo.SiteDir, false), TbSiteDir.Text))
		        {
		            var list = DataProvider.SiteDao.GetLowerSiteDirList(SiteInfo.ParentId);
		            if (list.IndexOf(TbSiteDir.Text.ToLower()) != -1)
		            {
		                FailMessage("站点修改失败，已存在相同的发布路径！");
		                return;
		            }

                    var parentPsPath = WebConfigUtils.PhysicalApplicationPath;
                    if (SiteInfo.ParentId > 0)
                    {
                        var parentSiteInfo = SiteManager.GetSiteInfo(SiteInfo.ParentId);
                        parentPsPath = PathUtility.GetSitePath(parentSiteInfo);
                    }
                    DirectoryUtility.ChangeSiteDir(parentPsPath, SiteInfo.SiteDir, TbSiteDir.Text);
                }

		        if (PhParentId.Visible && SiteInfo.ParentId != TranslateUtils.ToInt(DdlParentId.SelectedValue))
		        {
		            var newParentId = TranslateUtils.ToInt(DdlParentId.SelectedValue);
		            var list = DataProvider.SiteDao.GetLowerSiteDirList(newParentId);
		            if (list.IndexOf(TbSiteDir.Text.ToLower()) != -1)
		            {
		                FailMessage("站点修改失败，已存在相同的发布路径！");
		                return;
		            }

                    DirectoryUtility.ChangeParentSite(SiteInfo.ParentId, TranslateUtils.ToInt(DdlParentId.SelectedValue), SiteId, TbSiteDir.Text);
                    SiteInfo.ParentId = newParentId;
                }

		        SiteInfo.SiteDir = TbSiteDir.Text;
		    }

            DataProvider.SiteDao.Update(SiteInfo);
            if (isTableChanged)
            {
                DataProvider.ChannelDao.UpdateContentNum(SiteInfo);
            }

            AuthRequest.AddAdminLog("修改站点属性", $"站点:{SiteInfo.SiteName}");

            SuccessMessage("站点修改成功！");
            AddWaitAndRedirectScript(PageSite.GetRedirectUrl());
        }

        public void Return_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageSite.GetRedirectUrl());
        }
    }
}

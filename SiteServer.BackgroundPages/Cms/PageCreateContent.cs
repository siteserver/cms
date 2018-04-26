using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.BackgroundPages.Settings;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Create;
using SiteServer.Plugin;

namespace SiteServer.BackgroundPages.Cms
{
    public class PageCreateContent : BasePageCms
    {
        public ListBox LbChannelIdList;
        public DropDownList DdlScope;

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("siteId");

            if (IsPostBack) return;

            VerifySitePermissions(ConfigManager.WebSitePermissions.Create);

            var listitem = new ListItem("所有选中的栏目", "All");
            DdlScope.Items.Add(listitem);
            listitem = new ListItem("一个月内更新的内容", "Month");
            DdlScope.Items.Add(listitem);
            listitem = new ListItem("一天内更新的内容", "Day");
            DdlScope.Items.Add(listitem);
            listitem = new ListItem("2小时内更新的内容", "2Hour");
            DdlScope.Items.Add(listitem);

            ChannelManager.AddListItems(LbChannelIdList.Items, SiteInfo, false, true, AuthRequest.AdminPermissions);
        }


        public void Create_OnClick(object sender, EventArgs e)
        {
            if (!Page.IsPostBack || !Page.IsValid) return;

            var channelIdList = new List<int>();
            var selectedChannelIdArrayList = ControlUtils.GetSelectedListControlValueArrayList(LbChannelIdList);

            var tableName = SiteInfo.TableName;

            if (DdlScope.SelectedValue == "Month")
            {
                var lastEditList = DataProvider.ContentDao.GetChannelIdListCheckedByLastEditDateHour(tableName, SiteId, 720);
                foreach (var channelId in lastEditList)
                {
                    if (selectedChannelIdArrayList.Contains(channelId.ToString()))
                    {
                        channelIdList.Add(channelId);
                    }
                }
            }
            else if (DdlScope.SelectedValue == "Day")
            {
                var lastEditList = DataProvider.ContentDao.GetChannelIdListCheckedByLastEditDateHour(tableName, SiteId, 24);
                foreach (var channelId in lastEditList)
                {
                    if (selectedChannelIdArrayList.Contains(channelId.ToString()))
                    {
                        channelIdList.Add(channelId);
                    }
                }
            }
            else if (DdlScope.SelectedValue == "2Hour")
            {
                var lastEditList = DataProvider.ContentDao.GetChannelIdListCheckedByLastEditDateHour(tableName, SiteId, 2);
                foreach (var channelId in lastEditList)
                {
                    if (selectedChannelIdArrayList.Contains(channelId.ToString()))
                    {
                        channelIdList.Add(channelId);
                    }
                }
            }
            else
            {
                channelIdList = TranslateUtils.StringCollectionToIntList(TranslateUtils.ObjectCollectionToString(selectedChannelIdArrayList));
            }


            if (channelIdList.Count == 0)
            {
                FailMessage("请首先选中希望生成内容页面的栏目！");
                return;
            }

            foreach (var channelId in channelIdList)
            {
                CreateManager.CreateAllContent(SiteId, channelId);
            }

            PageCreateStatus.Redirect(SiteId);
        }
    }
}

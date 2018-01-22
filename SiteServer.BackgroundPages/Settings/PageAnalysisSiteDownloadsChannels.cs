using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using SiteServer.Utils;
using SiteServer.Utils.Model;
using SiteServer.Utils.Model.Enumerations;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.CMS.Provider;

namespace SiteServer.BackgroundPages.Settings
{
    public class PageAnalysisSiteDownloadsChannels : BasePageCms
    {
        public DropDownList DdlPublishmentSystemId;
        public Repeater RptContents;
        public SqlPager SpContents;

        public string StrArray { get; set; }

        private readonly Hashtable _nodeNameNavigations = new Hashtable();
        private readonly Hashtable _horizentalHashtable = new Hashtable();
        private readonly Hashtable _verticalHashtable = new Hashtable();
        private readonly List<int> _contentIdList = new List<int>();
        private readonly Hashtable _xHashtable = new Hashtable();
        private readonly Hashtable _yHashtableDownload = new Hashtable();
        private const string YTypeDownload = "YType_Download";

        public static string GetRedirectUrl(int publishmentSystemId)
        {
            return PageUtils.GetSettingsUrl(nameof(PageAnalysisSiteDownloadsChannels), new NameValueCollection
            {
                {"publishmentSystemID", publishmentSystemId.ToString()}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;

            PageUtils.CheckRequestParameter("PublishmentSystemID");

            SpContents.ControlToPaginate = RptContents;
            RptContents.ItemDataBound += RptContents_ItemDataBound;
            SpContents.ItemsPerPage = PublishmentSystemInfo.Additional.PageSize;

            SpContents.SelectCommand = DataProvider.ContentDao.GetSelectCommendByDownloads(PublishmentSystemInfo.AuxiliaryTableForContent, PublishmentSystemId);

            SpContents.SortField = ContentDao.SortFieldName;
            SpContents.SortMode = SortMode.DESC;

            if (IsPostBack) return;

            VerifyAdministratorPermissions(AppManager.Permissions.Settings.Chart);

            DdlPublishmentSystemId.Items.Add(new ListItem("<<全部站点>>", "0"));
            var publishmentSystemIdList = PublishmentSystemManager.GetPublishmentSystemIdListOrderByLevel();
            foreach (var publishmentSystemId in publishmentSystemIdList)
            {
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
                DdlPublishmentSystemId.Items.Add(new ListItem(publishmentSystemInfo.PublishmentSystemName, publishmentSystemId.ToString()));
            }
            ControlUtils.SelectSingleItem(DdlPublishmentSystemId, PublishmentSystemId.ToString());

            SpContents.DataBind();

            foreach (var contentId in _contentIdList)
            {
                var yValueDownload = GetYHashtable(contentId);
                var xValue = GetXHashtable(contentId);
                if (xValue.Length > 10) xValue = xValue.Substring(0, 10);

                StrArray += $@"
xArrayDownload.push('{xValue}');
                yArrayDownload.push('{yValueDownload}');";
            }
        }

        public void Analysis_OnClick(object sender, EventArgs e)
        {
            var publishmentSystemId = TranslateUtils.ToInt(DdlPublishmentSystemId.SelectedValue);
            PageUtils.Redirect(publishmentSystemId > 0
                ? GetRedirectUrl(publishmentSystemId)
                : PageAnalysisSiteDownloads.GetRedirectUrl());
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var ltlItemTitle = (Literal)e.Item.FindControl("ltlItemTitle");
            var ltlChannel = (Literal)e.Item.FindControl("ltlChannel");
            var ltlFileUrl = (Literal)e.Item.FindControl("ltlFileUrl");

            var contentInfo = new ContentInfo(e.Item.DataItem);

            ltlItemTitle.Text = WebUtils.GetContentTitle(PublishmentSystemInfo, contentInfo, PageUrl);

            string nodeNameNavigation;
            if (!_nodeNameNavigations.ContainsKey(contentInfo.NodeId))
            {
                nodeNameNavigation = NodeManager.GetNodeNameNavigation(PublishmentSystemId, contentInfo.NodeId);
                _nodeNameNavigations.Add(contentInfo.NodeId, nodeNameNavigation);
            }
            else
            {
                nodeNameNavigation = _nodeNameNavigations[contentInfo.NodeId] as string;
            }
            ltlChannel.Text = nodeNameNavigation;

            var fileUrl = contentInfo.GetString(BackgroundContentAttribute.FileUrl);
            if (!string.IsNullOrEmpty(fileUrl))
            {
                ltlFileUrl.Text =
                $@"<a href=""{PageUtility.ParseNavigationUrl(PublishmentSystemInfo, fileUrl, true)}"" target=""_blank"">{fileUrl}</a>";
            }

            //x轴信息
            SetXHashtable(contentInfo.Id, contentInfo.Title);
            //y轴信息
            SetYHashtable(contentInfo.Id, CountManager.GetCount(PublishmentSystemInfo.AuxiliaryTableForContent, contentInfo.Id.ToString(), ECountType.Download));
        }

        private string _pageUrl;
        private string PageUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_pageUrl))
                {
                    _pageUrl = PageUtils.GetSettingsUrl(nameof(PageAnalysisSiteDownloadsChannels), new NameValueCollection
                    {
                        {"PublishmentSystemID", PublishmentSystemId.ToString()}
                    });
                }
                return _pageUrl;
            }
        }

        private void SetXHashtable(int contentId, string title)
        {
            if (!_xHashtable.ContainsKey(contentId))
            {
                _xHashtable.Add(contentId, title);
            }
            if (!_contentIdList.Contains(contentId))
            {
                _contentIdList.Add(contentId);
            }
        }

        private string GetXHashtable(int contentId)
        {
            return _xHashtable.ContainsKey(contentId) ? _xHashtable[contentId].ToString() : string.Empty;
        }

        private void SetYHashtable(int contentId, int value)
        {

            if (!_yHashtableDownload.ContainsKey(contentId))
            {
                _yHashtableDownload.Add(contentId, value);
            }
            else
            {
                var num = TranslateUtils.ToInt(_yHashtableDownload[contentId].ToString());
                _yHashtableDownload[contentId] = num + value;
            }
            SetVertical(YTypeDownload, value);

            SetHorizental(contentId, value);
        }

        private string GetYHashtable(int contentId)
        {
            if (!_yHashtableDownload.ContainsKey(contentId)) return "0";
            var num = TranslateUtils.ToInt(_yHashtableDownload[contentId].ToString());
            return num.ToString();
        }

        private void SetHorizental(int contentId, int num)
        {
            if (_horizentalHashtable[contentId] == null)
            {
                _horizentalHashtable[contentId] = num;
            }
            else
            {
                var totalNum = (int)_horizentalHashtable[contentId];
                _horizentalHashtable[contentId] = totalNum + num;
            }
        }

        private void SetVertical(string type, int num)
        {
            if (_verticalHashtable[type] == null)
            {
                _verticalHashtable[type] = num;
            }
            else
            {
                var totalNum = (int)_verticalHashtable[type];
                _verticalHashtable[type] = totalNum + num;
            }
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using System.Collections.Specialized;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;

namespace SiteServer.BackgroundPages.Analysis
{
    public class PageAnalysisContentDownloads : BasePageCms
    {
        public Literal LtlArray;
        public Repeater RptContents;
        public SqlPager SpContents;

        private readonly Hashtable _nodeNameNavigations = new Hashtable();
        private readonly Hashtable _horizentalHashtable = new Hashtable();
        private readonly Hashtable _verticalHashtable = new Hashtable();
        private readonly List<int> _contentIdList = new List<int>();
        private readonly Hashtable _xHashtable = new Hashtable();
        private readonly Hashtable _yHashtableDownload = new Hashtable();
        private const string YTypeDownload = "YType_Download";

        public static string GetRedirectUrl(int publishmentSystemId, string returnUrl)
        {
            return PageUtils.GetAnalysisUrl(nameof(PageAnalysisContentDownloads), new NameValueCollection
                {
                    {"returnUrl", StringUtils.ValueToUrl(returnUrl)},
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

            SpContents.SelectCommand = DataProvider.BackgroundContentDao.GetSelectCommendByDownloads(PublishmentSystemInfo.AuxiliaryTableForContent, PublishmentSystemId);

            SpContents.SortField = BaiRongDataProvider.ContentDao.GetSortFieldName();
            SpContents.SortMode = SortMode.DESC;

            if (IsPostBack) return;

            if (PublishmentSystemId > 0)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdFunction, AppManager.Cms.LeftMenu.Function.IdSiteAnalysis, "文件下载量排名", AppManager.Cms.Permission.WebSite.SiteAnalysis);
            }
            else
            {
                BreadCrumbAnalysis(AppManager.Analysis.LeftMenu.Chart, "文件下载统计", AppManager.Analysis.Permission.AnalysisChart);
            }

            SpContents.DataBind();

            foreach (var contentId in _contentIdList)
            {
                var yValueDownload = GetYHashtable(contentId);
                var xValue = GetXHashtable(contentId);
                if (xValue.Length > 10) xValue = xValue.Substring(0, 10);

                LtlArray.Text += $@"
xArrayDownload.push('{xValue}');
                yArrayDownload.push('{yValueDownload}');";
            }
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var ltlItemTitle = (Literal)e.Item.FindControl("ltlItemTitle");
            var ltlChannel = (Literal)e.Item.FindControl("ltlChannel");
            var ltlFileUrl = (Literal)e.Item.FindControl("ltlFileUrl");

            var contentInfo = new BackgroundContentInfo(e.Item.DataItem);

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

            ltlFileUrl.Text =
                $@"<a href=""{PageUtility.ParseNavigationUrl(PublishmentSystemInfo, contentInfo.FileUrl)}"" target=""_blank"">{contentInfo
                    .FileUrl}</a>";

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
                    _pageUrl = PageUtils.GetAnalysisUrl(nameof(PageAnalysisContentDownloads), new NameValueCollection
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

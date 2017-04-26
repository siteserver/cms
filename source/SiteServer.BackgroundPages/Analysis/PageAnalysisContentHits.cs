using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Attributes;
using SiteServer.BackgroundPages.Controls;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Analysis
{
    public class PageAnalysisContentHits : BasePageCms
    {
        public Literal LtlArray;
        public Repeater RptContents;
        public SqlPager SpContents;

        private string _pageUrl;
        private readonly Hashtable _nodeNameNavigations = new Hashtable();
        private readonly Hashtable _horizentalHashtable = new Hashtable();
        private readonly Hashtable _verticalHashtable = new Hashtable();
        private readonly List<int> _contentIdList = new List<int>();
        private readonly Hashtable _xHashtable = new Hashtable();
        private readonly Hashtable _yHashtableHits = new Hashtable();
        private readonly Hashtable _yHashtableHitsDay = new Hashtable();
        private readonly Hashtable _yHashtableHitsWeek = new Hashtable();
        private readonly Hashtable _yHashtableHitsMonth = new Hashtable();
        private const string YTypeHits = "YType_Hits";
        private const string YTypeHitsDay = "YType_HitsDay";
        private const string YTypeHitsWeek = "YType_HitsWeek";
        private const string YTypeHitsMonth = "YType_HitsMonth";

        public static string GetRedirectUrl(int publishmentSystemId, string returnUrl)
        {
            return PageUtils.GetAnalysisUrl(nameof(PageAnalysisContentHits), new NameValueCollection
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

            SpContents.SelectCommand = BaiRongDataProvider.ContentDao.GetSelectCommendByHitsAnalysis(PublishmentSystemInfo.AuxiliaryTableForContent, PublishmentSystemId);

            SpContents.SortField = ContentAttribute.Hits;
            SpContents.SortMode = SortMode.DESC;

            _pageUrl = PageUtils.GetAnalysisUrl(nameof(PageAnalysisContentHits), new NameValueCollection
            {
                {"PublishmentSystemID", PublishmentSystemId.ToString()}
            });

            if (IsPostBack) return;

            if (PublishmentSystemId > 0)
            {
                BreadCrumb(AppManager.Cms.LeftMenu.IdFunction, AppManager.Cms.LeftMenu.Function.IdSiteAnalysis, "内容点击量排名", AppManager.Cms.Permission.WebSite.SiteAnalysis);
            }
            else
            {
                BreadCrumbAnalysis(AppManager.Analysis.LeftMenu.Chart, "内容点击量排名", AppManager.Analysis.Permission.AnalysisChart);
            }

            SpContents.DataBind();

            foreach (var contentId in _contentIdList)
            {
                var yValueHits = GetYHashtable(contentId, YTypeHits);
                var yValueHitsDay = GetYHashtable(contentId, YTypeHitsDay);
                var yValueHitsWeek = GetYHashtable(contentId, YTypeHitsWeek);
                var yValueHitsMonth = GetYHashtable(contentId, YTypeHitsMonth);
                var xValue = GetXHashtable(contentId);
                if (xValue.Length > 10) xValue = xValue.Substring(0, 10);

                LtlArray.Text += $@"
xArrayHits.push('{xValue}');
yArrayHits.push('{yValueHits}');
yArrayHitsDay.push('{yValueHitsDay}');
yArrayHitsWeek.push('{yValueHitsWeek}');
yArrayHitsMonth.push('{yValueHitsMonth}');
";
            }
        }

        private void RptContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

            var ltlItemTitle = (Literal)e.Item.FindControl("ltlItemTitle");
            var ltlChannel = (Literal)e.Item.FindControl("ltlChannel");
            var ltlHits = (Literal)e.Item.FindControl("ltlHits");
            var ltlHitsByDay = (Literal)e.Item.FindControl("ltlHitsByDay");
            var ltlHitsByWeek = (Literal)e.Item.FindControl("ltlHitsByWeek");
            var ltlHitsByMonth = (Literal)e.Item.FindControl("ltlHitsByMonth");
            var ltlLastHitsDate = (Literal)e.Item.FindControl("ltlLastHitsDate");

            var contentInfo = new ContentInfo(e.Item.DataItem);

            ltlItemTitle.Text = WebUtils.GetContentTitle(PublishmentSystemInfo, contentInfo, _pageUrl);

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

            ltlHits.Text = contentInfo.Hits.ToString();
            ltlHitsByDay.Text = contentInfo.HitsByDay.ToString();
            ltlHitsByMonth.Text = contentInfo.HitsByMonth.ToString();
            ltlHitsByWeek.Text = contentInfo.HitsByWeek.ToString();
            ltlLastHitsDate.Text = DateUtils.GetDateAndTimeString(contentInfo.LastHitsDate);


            //x轴信息
            SetXHashtable(contentInfo.Id, contentInfo.Title);
            //y轴信息
            SetYHashtable(contentInfo.Id, contentInfo.Hits, YTypeHits);
            SetYHashtable(contentInfo.Id, contentInfo.HitsByDay, YTypeHitsDay);
            SetYHashtable(contentInfo.Id, contentInfo.HitsByWeek, YTypeHitsWeek);
            SetYHashtable(contentInfo.Id, contentInfo.HitsByMonth, YTypeHitsMonth);
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
            if (_xHashtable.ContainsKey(contentId))
            {
                return _xHashtable[contentId].ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        private void SetYHashtable(int publishemtSystemId, int value, string yType)
        {
            switch (yType)
            {
                case YTypeHits:
                    if (!_yHashtableHits.ContainsKey(publishemtSystemId))
                    {
                        _yHashtableHits.Add(publishemtSystemId, value);
                    }
                    else
                    {
                        var num = TranslateUtils.ToInt(_yHashtableHits[publishemtSystemId].ToString());
                        _yHashtableHits[publishemtSystemId] = num + value;
                    }
                    SetVertical(YTypeHits, value);
                    break;
                case YTypeHitsDay:
                    if (!_yHashtableHitsDay.ContainsKey(publishemtSystemId))
                    {
                        _yHashtableHitsDay.Add(publishemtSystemId, value);
                    }
                    else
                    {
                        var num = TranslateUtils.ToInt(_yHashtableHitsDay[publishemtSystemId].ToString());
                        _yHashtableHitsDay[publishemtSystemId] = num + value;
                    }
                    SetVertical(YTypeHitsDay, value);
                    break;
                case YTypeHitsWeek:
                    if (!_yHashtableHitsWeek.ContainsKey(publishemtSystemId))
                    {
                        _yHashtableHitsWeek.Add(publishemtSystemId, value);
                    }
                    else
                    {
                        var num = TranslateUtils.ToInt(_yHashtableHitsWeek[publishemtSystemId].ToString());
                        _yHashtableHitsWeek[publishemtSystemId] = num + value;
                    }
                    SetVertical(YTypeHitsWeek, value);
                    break;
                case YTypeHitsMonth:
                    if (!_yHashtableHitsMonth.ContainsKey(publishemtSystemId))
                    {
                        _yHashtableHitsMonth.Add(publishemtSystemId, value);
                    }
                    else
                    {
                        var num = TranslateUtils.ToInt(_yHashtableHitsMonth[publishemtSystemId].ToString());
                        _yHashtableHitsMonth[publishemtSystemId] = num + value;
                    }
                    SetVertical(YTypeHitsWeek, value);
                    break;
            }
            SetHorizental(publishemtSystemId, value);
        }

        private string GetYHashtable(int publishemtSystemId, string yType)
        {
            switch (yType)
            {
                case YTypeHits:
                    if (_yHashtableHits.ContainsKey(publishemtSystemId))
                    {
                        var num = TranslateUtils.ToInt(_yHashtableHits[publishemtSystemId].ToString());
                        return num.ToString();
                    }
                    return "0";
                case YTypeHitsDay:
                    if (_yHashtableHitsDay.ContainsKey(publishemtSystemId))
                    {
                        var num = TranslateUtils.ToInt(_yHashtableHitsDay[publishemtSystemId].ToString());
                        return num.ToString();
                    }
                    return "0";
                case YTypeHitsWeek:
                    if (_yHashtableHitsWeek.ContainsKey(publishemtSystemId))
                    {
                        var num = TranslateUtils.ToInt(_yHashtableHitsWeek[publishemtSystemId].ToString());
                        return num.ToString();
                    }
                    return "0";
                case YTypeHitsMonth:
                    if (_yHashtableHitsMonth.ContainsKey(publishemtSystemId))
                    {
                        var num = TranslateUtils.ToInt(_yHashtableHitsMonth[publishemtSystemId].ToString());
                        return num.ToString();
                    }
                    return "0";
                default:
                    return "0";
            }
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

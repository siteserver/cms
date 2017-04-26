using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Controls;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Analysis
{
    public class PageAnalysisSiteHits : BasePageCms
    {
        public DateTimeTextBox StartDate;
        public DateTimeTextBox EndDate;
        public Repeater RpContents;
        public Literal LtlArray;
        public Literal LtlVertical;

        private readonly Hashtable _horizentalHashtable = new Hashtable();
        private readonly Hashtable _verticalHashtable = new Hashtable();
        private readonly List<int> _publishmentSystemIdList = new List<int>();
        private readonly Hashtable _xHashtable = new Hashtable();
        private readonly Hashtable _yHashtableHits = new Hashtable();
        private const string YTypeHits = "YType_Hits";

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            if (IsPostBack) return;

            BreadCrumbAnalysis(AppManager.Analysis.LeftMenu.Chart, "站点访问量统计", AppManager.Analysis.Permission.AnalysisChart);

            StartDate.Text = DateUtils.GetDateAndTimeString(DateTime.Now.AddMonths(-1));
            EndDate.Now = true;

            BindGrid();

            foreach (var key in _publishmentSystemIdList)
            {
                var yValueHits = GetYHashtable(key);
                if (yValueHits != "0")
                {
                    LtlArray.Text += $@"
xArrayHits.push('{GetXHashtable(key)}');
yArrayHits.push('{yValueHits}');";
                }
            }
            LtlVertical.Text = GetVerticalTotalNum();
        }

        public void BindGrid()
        {
            var ie = DataProvider.PublishmentSystemDao.GetDataSource().GetEnumerator();
            while (ie.MoveNext())
            {
                var publishmentSystemId = SqlUtils.EvalInt(ie.Current, "PublishmentSystemID");
                var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);

                var key = publishmentSystemInfo.PublishmentSystemId;
                //x轴信息
                SetXHashtable(key, publishmentSystemInfo.PublishmentSystemName);
                //y轴信息
                SetYHashtable(key, DataProvider.TrackingDao.GetHitsCountOfPublishmentSystem(publishmentSystemInfo.PublishmentSystemId, TranslateUtils.ToDateTime(StartDate.Text), TranslateUtils.ToDateTime(EndDate.Text)));
            }

            RpContents.DataSource = DataProvider.PublishmentSystemDao.GetDataSource();
            RpContents.ItemDataBound += rpContents_ItemDataBound;
            RpContents.DataBind();
        }

        private void rpContents_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;
            var publishmentSystemId = SqlUtils.EvalInt(e.Item.DataItem, "PublishmentSystemID");
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);

            var ltlPublishmentSystemName = (Literal)e.Item.FindControl("ltlPublishmentSystemName");
            var ltlHitsNum = (Literal)e.Item.FindControl("ltlHitsNum");

            ltlPublishmentSystemName.Text = publishmentSystemInfo.PublishmentSystemName + "&nbsp;" +
                                            EPublishmentSystemTypeUtils.GetIconHtml(publishmentSystemInfo.PublishmentSystemType);
            ltlHitsNum.Text = GetYHashtable(publishmentSystemId);
        }

        public void Analysis_OnClick(object sender, EventArgs e)
        {
            BindGrid();
        }

        private void SetXHashtable(int publishmentSystemId, string publishmentSystemName)
        {
            if (!_xHashtable.ContainsKey(publishmentSystemId))
            {
                _xHashtable.Add(publishmentSystemId, publishmentSystemName);
            }
            if (!_publishmentSystemIdList.Contains(publishmentSystemId))
            {
                _publishmentSystemIdList.Add(publishmentSystemId);
            }
            _publishmentSystemIdList.Sort();
            _publishmentSystemIdList.Reverse();
        }

        private string GetXHashtable(int publishmentSystemId)
        {
            return _xHashtable.ContainsKey(publishmentSystemId) ? _xHashtable[publishmentSystemId].ToString() : string.Empty;
        }

        private void SetYHashtable(int publishemtSystemId, int value)
        {

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

            SetHorizental(publishemtSystemId, value);
        }

        private string GetYHashtable(int publishemtSystemId)
        {
            if (_yHashtableHits.ContainsKey(publishemtSystemId))
            {
                var num = TranslateUtils.ToInt(_yHashtableHits[publishemtSystemId].ToString());
                return num.ToString();
            }
            return "0";
        }

        private void SetHorizental(int publishmentSystemId, int num)
        {
            if (_horizentalHashtable[publishmentSystemId] == null)
            {
                _horizentalHashtable[publishmentSystemId] = num;
            }
            else
            {
                var totalNum = (int)_horizentalHashtable[publishmentSystemId];
                _horizentalHashtable[publishmentSystemId] = totalNum + num;
            }
        }

        private string GetHorizental(int publishmentSystemId)
        {
            if (_horizentalHashtable[publishmentSystemId] == null) return "0";

            var num = TranslateUtils.ToInt(_horizentalHashtable[publishmentSystemId].ToString());
            return (num == 0) ? "0" : $"<strong>{num}</strong>";
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

        private string GetVertical(string type)
        {
            if (_verticalHashtable[type] == null) return "0";

            var num = TranslateUtils.ToInt(_verticalHashtable[type].ToString());
            return (num == 0) ? "0" : $"<strong>{num}</strong>";
        }

        private string GetVerticalTotalNum()
        {
            var totalNum = 0;
            foreach (int num in _verticalHashtable.Values)
            {
                totalNum += num;
            }
            return totalNum == 0 ? "0" : $"<strong>{totalNum}</strong>";
        }
    }
}

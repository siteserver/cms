using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Controls;
using SiteServer.CMS.Core;

namespace SiteServer.BackgroundPages.Analysis
{
    public class PageAnalysisSite : BasePageCms
    {
        public DateTimeTextBox StartDate;
        public DateTimeTextBox EndDate;
        public Repeater RpContents;
        public Literal LtlArray;
        public Literal LtlVerticalNew;
        public Literal LtlVerticalUpdate;
        public Literal LtlVerticalRemrk;
        public Literal LtlVerticalTotalNum;

        //总数
        private readonly Hashtable _horizentalHashtable = new Hashtable();
        private readonly Hashtable _verticalHashtable = new Hashtable();
        //sort key
        private readonly List<int> _publishmentSystemIdList = new List<int>();
        //x
        private readonly Hashtable _xHashtable = new Hashtable();
        //y
        private readonly Hashtable _yHashtableNew = new Hashtable();
        private readonly Hashtable _yHashtableUpdate = new Hashtable();
        private readonly Hashtable _yHashtableRemark = new Hashtable();
        //y轴类型
        private const string YTypeNew = "YType_New";
        private const string YTypeUpdate = "YType_Update";
        private const string YTypeRemrk = "YType_Remrk";

        public static string GetRedirectUrl(string returnUrl)
        {
            return PageUtils.GetAnalysisUrl(nameof(PageAnalysisSite), new NameValueCollection
            {
                {"returnUrl", StringUtils.ValueToUrl(returnUrl)}
            });
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            if (IsPostBack) return;

            BreadCrumbAnalysis(AppManager.Analysis.LeftMenu.Chart, "站点数据统计", AppManager.Analysis.Permission.AnalysisChart);

            StartDate.Text = DateUtils.GetDateAndTimeString(DateTime.Now.AddMonths(-1));
            EndDate.Now = true;

            BindGrid();

            foreach (var key in _publishmentSystemIdList)
            {
                var yValueNew = GetYHashtable(key, YTypeNew);
                var yValueUpdate = GetYHashtable(key, YTypeUpdate);
                var yValueRemark = GetYHashtable(key, YTypeRemrk);

                if (yValueNew != "0")
                {
                    LtlArray.Text += $@"
xArrayNew.push('{GetXHashtable(key)}');
yArrayNew.push('{yValueNew}');";
                }
                if (yValueUpdate != "0")
                {
                    LtlArray.Text += $@"
xArrayUpdate.push('{GetXHashtable(key)}');
yArrayUpdate.push('{yValueUpdate}');";
                }
                if (yValueRemark != "0")
                {
                    LtlArray.Text += $@"
xArrayRemark.push('{GetXHashtable(key)}');
yArrayRemark.push('{yValueRemark}');";
                }
            }

            LtlVerticalNew.Text = GetVertical(YTypeNew);
            LtlVerticalUpdate.Text = GetVertical(YTypeUpdate);
            LtlVerticalRemrk.Text = GetVertical(YTypeRemrk);
            LtlVerticalTotalNum.Text = GetVerticalTotalNum();
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
                SetYHashtable(key, DataProvider.ContentDao.GetCountOfContentAdd(publishmentSystemInfo.AuxiliaryTableForContent, publishmentSystemInfo.PublishmentSystemId, publishmentSystemInfo.PublishmentSystemId, TranslateUtils.ToDateTime(StartDate.Text), TranslateUtils.ToDateTime(EndDate.Text), string.Empty), YTypeNew);
                SetYHashtable(key, DataProvider.ContentDao.GetCountOfContentUpdate(publishmentSystemInfo.AuxiliaryTableForContent, publishmentSystemInfo.PublishmentSystemId, publishmentSystemInfo.PublishmentSystemId, TranslateUtils.ToDateTime(StartDate.Text), TranslateUtils.ToDateTime(EndDate.Text), string.Empty), YTypeUpdate);
                SetYHashtable(key, DataProvider.CommentDao.GetCountChecked(publishmentSystemInfo.PublishmentSystemId, TranslateUtils.ToDateTime(StartDate.Text), TranslateUtils.ToDateTime(EndDate.Text)), YTypeRemrk);
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
            var ltlNewContentNum = (Literal)e.Item.FindControl("ltlNewContentNum");
            var ltlUpdateContentNum = (Literal)e.Item.FindControl("ltlUpdateContentNum");
            var ltlNewRemarkNum = (Literal)e.Item.FindControl("ltlNewRemarkNum");
            var ltlTotalNum = (Literal)e.Item.FindControl("ltlTotalNum");

            ltlPublishmentSystemName.Text = publishmentSystemInfo.PublishmentSystemName + "&nbsp;" + EPublishmentSystemTypeUtils.GetIconHtml(publishmentSystemInfo.PublishmentSystemType);
            ltlNewContentNum.Text = GetYHashtable(publishmentSystemId, YTypeNew);
            ltlUpdateContentNum.Text = GetYHashtable(publishmentSystemId, YTypeUpdate);
            ltlNewRemarkNum.Text = GetYHashtable(publishmentSystemId, YTypeRemrk);
            ltlTotalNum.Text = GetHorizental(publishmentSystemId);
        }

        public void Analysis_OnClick(object sender, EventArgs e)
        {
            BindGrid();
        }

        /// <summary>
        /// 设置x轴数据
        /// </summary>
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

        /// <summary>
        /// 获取x轴数据
        /// </summary>
        private string GetXHashtable(int publishmentSystemId)
        {
            return _xHashtable.ContainsKey(publishmentSystemId) ? _xHashtable[publishmentSystemId].ToString() : string.Empty;
        }

        /// <summary>
        /// 设置y轴数据
        /// </summary>
        private void SetYHashtable(int publishemtSystemId, int value, string yType)
        {
            switch (yType)
            {
                case YTypeNew:
                    if (!_yHashtableNew.ContainsKey(publishemtSystemId))
                    {
                        _yHashtableNew.Add(publishemtSystemId, value);
                    }
                    else
                    {
                        var num = TranslateUtils.ToInt(_yHashtableNew[publishemtSystemId].ToString());
                        _yHashtableNew[publishemtSystemId] = num + value;
                    }
                    SetVertical(YTypeNew, value);
                    break;
                case YTypeUpdate:
                    if (!_yHashtableUpdate.ContainsKey(publishemtSystemId))
                    {
                        _yHashtableUpdate.Add(publishemtSystemId, value);
                    }
                    else
                    {
                        var num = TranslateUtils.ToInt(_yHashtableUpdate[publishemtSystemId].ToString());
                        _yHashtableUpdate[publishemtSystemId] = num + value;
                    }
                    SetVertical(YTypeUpdate, value);
                    break;
                case YTypeRemrk:
                    if (!_yHashtableRemark.ContainsKey(publishemtSystemId))
                    {
                        _yHashtableRemark.Add(publishemtSystemId, value);
                    }
                    else
                    {
                        var num = TranslateUtils.ToInt(_yHashtableRemark[publishemtSystemId].ToString());
                        _yHashtableRemark[publishemtSystemId] = num + value;
                    }
                    SetVertical(YTypeRemrk, value);
                    break;
            }
            SetHorizental(publishemtSystemId, value);
        }

        /// <summary>
        /// 获取y轴数据
        /// </summary>
        private string GetYHashtable(int publishemtSystemId, string yType)
        {
            switch (yType)
            {
                case YTypeNew:
                    if (_yHashtableNew.ContainsKey(publishemtSystemId))
                    {
                        var num = TranslateUtils.ToInt(_yHashtableNew[publishemtSystemId].ToString());
                        return num.ToString();
                    }
                    return "0";
                case YTypeUpdate:
                    if (_yHashtableUpdate.ContainsKey(publishemtSystemId))
                    {
                        var num = TranslateUtils.ToInt(_yHashtableUpdate[publishemtSystemId].ToString());
                        return num.ToString();
                    }
                    return "0";
                case YTypeRemrk:
                    if (_yHashtableRemark.ContainsKey(publishemtSystemId))
                    {
                        var num = TranslateUtils.ToInt(_yHashtableRemark[publishemtSystemId].ToString());
                        return num.ToString();
                    }
                    return "0";

                default:
                    return "0";
            }
        }

        /// <summary>
        /// 设置y总数
        /// </summary>
        /// <param name="publishmentSystemId"></param>
        /// <param name="num"></param>
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

        /// <summary>
        /// 获取y总数
        /// </summary>
        /// <param name="publishmentSystemId"></param>
        /// <returns></returns>
        private string GetHorizental(int publishmentSystemId)
        {
            if (_horizentalHashtable[publishmentSystemId] != null)
            {
                var num = TranslateUtils.ToInt(_horizentalHashtable[publishmentSystemId].ToString());
                return (num == 0) ? "0" : $"<strong>{num}</strong>";
            }
            return "0";
        }

        /// <summary>
        /// 设置type总数
        /// </summary>
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

        /// <summary>
        /// 获取type总数
        /// </summary>
        private string GetVertical(string type)
        {
            if (_verticalHashtable[type] != null)
            {
                var num = TranslateUtils.ToInt(_verticalHashtable[type].ToString());
                return num == 0 ? "0" : $"<strong>{num}</strong>";
            }
            return "0";
        }

        private string GetVerticalTotalNum()
        {
            var totalNum = 0;
            foreach (int num in _verticalHashtable.Values)
            {
                totalNum += num;
            }
            return (totalNum == 0) ? "0" : $"<strong>{totalNum}</strong>";
        }
    }
}

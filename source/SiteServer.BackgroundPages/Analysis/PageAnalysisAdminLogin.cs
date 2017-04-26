using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Controls;

namespace SiteServer.BackgroundPages.Analysis
{
    public class PageAnalysisAdminLogin : BasePage
    {
        public Literal LtlPageTitle1;
        public Literal LtlPageTitle2;
        public DateTimeTextBox TbDateFrom;
        public DateTimeTextBox TbDateTo;
        public DropDownList DdlXType;
        public Literal LtlArray1;
        public Literal LtlArray2;

        //管理员登录（按日期）
        private readonly Dictionary<int, int> _adminNumDictionaryDay = new Dictionary<int, int>();
        //管理员登录（按用户）
        private int _maxAdminNum;
        private int _count = 30;
        private EStatictisXType _xType = EStatictisXType.Day;

        public double GetAccessNum(int index)
        {
            if (_maxAdminNum <= 0) return 0;
            var accessNum = Convert.ToDouble(_maxAdminNum) * Convert.ToDouble(index) / 8;
            accessNum = Math.Round(accessNum, 2);
            return accessNum;
        }

        public string GetGraphicHtml(int index)
        {
            if (index <= 0 || index > _count || !_adminNumDictionaryDay.ContainsKey(index)) return string.Empty;
            var accessNum = _adminNumDictionaryDay[index];
            var datetime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);

            var xNum = 0;
            if (Equals(_xType, EStatictisXType.Day))
            {
                datetime = datetime.AddDays(-(_count - index));
                xNum = datetime.Day;
            }
            else if (Equals(_xType, EStatictisXType.Month))
            {
                datetime = datetime.AddMonths(-(_count - index));
                xNum = datetime.Month;
            }
            else if (Equals(_xType, EStatictisXType.Year))
            {
                datetime = datetime.AddYears(-(_count - index));
                xNum = datetime.Year;
            }

            double height = 0;
            if (_maxAdminNum > 0)
            {
                height = (Convert.ToDouble(accessNum) / Convert.ToDouble(_maxAdminNum)) * 200.0;
            }
            string html =
                $"<IMG title=登录次数：{accessNum} height={height} style=height:{height}px src=../pic/tracker_bar.gif width=16><BR>{xNum}";
            return html;
        }

        private string GetGraphicX(int index)
        {
            var xNum = 0;
            var datetime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            if (Equals(_xType, EStatictisXType.Day))
            {
                datetime = datetime.AddDays(-(_count - index));
                xNum = datetime.Day;
            }
            else if (Equals(_xType, EStatictisXType.Month))
            {
                datetime = datetime.AddMonths(-(_count - index));
                xNum = datetime.Month;
            }
            else if (Equals(_xType, EStatictisXType.Year))
            {
                datetime = datetime.AddYears(-(_count - index));
                xNum = datetime.Year;
            }
            return xNum.ToString();
        }

        private string GetGraphicY(int index)
        {
            if (index <= 0 || index > _count || !_adminNumDictionaryDay.ContainsKey(index)) return string.Empty;
            var accessNum = _adminNumDictionaryDay[index];
            return accessNum.ToString();
        }

        private string GetGraphicYUser(Dictionary<string, int> adminNumDictionaryName, string key)
        {
            if (!string.IsNullOrEmpty(key) && adminNumDictionaryName.ContainsKey(key))
            {
                var accessNum = adminNumDictionaryName[key];
                return accessNum.ToString();
            }
            return string.Empty;
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            if (IsPostBack) return;

            BreadCrumbAnalysis(AppManager.Analysis.LeftMenu.Chart, "管理员登录统计", AppManager.Analysis.Permission.AnalysisChart);

            LtlPageTitle1.Text = $"管理员登录最近{_count}{EStatictisXTypeUtils.GetText(EStatictisXTypeUtils.GetEnumType(Body.GetQueryString("XType")))}分配图表（按日期统计）";
            LtlPageTitle2.Text = $"管理员登录最近{_count}{EStatictisXTypeUtils.GetText(EStatictisXTypeUtils.GetEnumType(Body.GetQueryString("XType")))}分配图表（按管理员统计）";

            EStatictisXTypeUtils.AddListItems(DdlXType);

            _xType = EStatictisXTypeUtils.GetEnumType(Body.GetQueryString("XType"));

            if (Equals(_xType, EStatictisXType.Day))
            {
                _count = 30;
            }
            else if (Equals(_xType, EStatictisXType.Month))
            {
                _count = 12;
            }
            else if (Equals(_xType, EStatictisXType.Year))
            {
                _count = 10;
            }


            TbDateFrom.Text = Body.GetQueryString("DateFrom");
            TbDateTo.Text = Body.GetQueryString("DateTo");
            DdlXType.SelectedValue = EStatictisXTypeUtils.GetValue(_xType);

            //管理员登录量统计，按照日期
            var trackingDayDictionary = BaiRongDataProvider.LogDao.GetAdminLoginDictionaryByDate(TranslateUtils.ToDateTime(Body.GetQueryString("DateFrom")), TranslateUtils.ToDateTime(Body.GetQueryString("DateTo"), DateTime.Now), EStatictisXTypeUtils.GetValue(_xType), LogInfo.AdminLogin);

            //管理员登录量统计，按照用户名
            var adminNumDictionaryName = BaiRongDataProvider.LogDao.GetAdminLoginDictionaryByName(TranslateUtils.ToDateTime(Body.GetQueryString("DateFrom")), TranslateUtils.ToDateTime(Body.GetQueryString("DateTo"), DateTime.Now), LogInfo.AdminLogin);

            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
            for (var i = 0; i < _count; i++)
            {
                var datetime = now.AddDays(-i);
                if (Equals(_xType, EStatictisXType.Day))
                {
                    now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 0, 0, 0);
                    datetime = now.AddDays(-i);
                }
                else if (Equals(_xType, EStatictisXType.Month))
                {
                    now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1, 0, 0, 0);
                    datetime = now.AddMonths(-i);
                }
                else if (Equals(_xType, EStatictisXType.Year))
                {
                    now = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0);
                    datetime = now.AddYears(-i);
                }

                var accessNum = 0;
                if (trackingDayDictionary.ContainsKey(datetime))
                {
                    accessNum = trackingDayDictionary[datetime];
                }
                _adminNumDictionaryDay.Add(_count - i, accessNum);
                if (accessNum > _maxAdminNum)
                {
                    _maxAdminNum = accessNum;
                }
            }

            for (var i = 1; i <= _count; i++)
            {
                LtlArray1.Text += $@"
xArray.push('{GetGraphicX(i)}');
yArray.push('{GetGraphicY(i)}');
";
            }

            foreach (var key in adminNumDictionaryName.Keys)
            {
                LtlArray2.Text += $@"
xArray.push('{key}');
yArray.push('{GetGraphicYUser(adminNumDictionaryName, key)}');
";
            }
        }

        public void Search_OnClick(object sender, EventArgs e)
        {
            PageUtils.Redirect(PageUrl);
        }

        private string _pageUrl;
        private string PageUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_pageUrl))
                {
                    _pageUrl = PageUtils.GetAnalysisUrl(nameof(PageAnalysisAdminLogin), new NameValueCollection
                    {
                        {"DateFrom", TbDateFrom.Text },
                        {"DateTo", TbDateTo.Text },
                        {"XType", DdlXType.SelectedValue }
                    });
                }
                return _pageUrl;
            }
        }
    }
}

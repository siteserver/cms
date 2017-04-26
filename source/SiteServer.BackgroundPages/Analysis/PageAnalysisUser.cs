using System;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI.WebControls;
using BaiRong.Core;
using BaiRong.Core.Model.Enumerations;
using SiteServer.BackgroundPages.Controls;

namespace SiteServer.BackgroundPages.Analysis
{
    public class PageAnalysisUser : BasePage
    {
        public Literal LtlPageTitle;
        public DateTimeTextBox TbDateFrom;
        public DateTimeTextBox TbDateTo;
        public Literal LtlArray;
        public DropDownList DdlXType;

        //用户数量
        private readonly Hashtable _userNumHashtable = new Hashtable();
        private int _maxUserNum;
        private int _count = 30;
        private EStatictisXType _xType = EStatictisXType.Day;

        public double GetAccessNum(int index)
        {
            if (_maxUserNum <= 0) return 0;

            var accessNum = Convert.ToDouble(_maxUserNum) * Convert.ToDouble(index) / 8;
            accessNum = Math.Round(accessNum, 2);
            return accessNum;
        }

        public string GetGraphicHtml(int index)
        {
            if (index <= 0 || index > _count) return string.Empty;
            var accessNum = (int)_userNumHashtable[index];
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
            if (_maxUserNum > 0)
            {
                height = (Convert.ToDouble(accessNum) / Convert.ToDouble(_maxUserNum)) * 200.0;
            }
            string html =
                $"<IMG title=访问量：{accessNum} height={height} style=height:{height}px src=../pic/tracker_bar.gif width=16><BR>{xNum}";
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
            if (index <= 0 || index > _count) return string.Empty;
            var accessNum = (int)_userNumHashtable[index];
            return accessNum.ToString();
        }

        public void Page_Load(object sender, EventArgs e)
        {
            if (IsForbidden) return;
            if (IsPostBack) return;

            BreadCrumbAnalysis(AppManager.Analysis.LeftMenu.Chart, "会员新增数据统计", AppManager.Analysis.Permission.AnalysisChart);
            LtlPageTitle.Text = $"用户增加最近{_count}{EStatictisXTypeUtils.GetText(EStatictisXTypeUtils.GetEnumType(Body.GetQueryString("XType")))}分配图表";

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

            //用户添加量统计
            var trackingDayDict = BaiRongDataProvider.UserDao.GetTrackingDictionary( TranslateUtils.ToDateTime(Body.GetQueryString("DateFrom")), TranslateUtils.ToDateTime(Body.GetQueryString("DateTo"), DateTime.Now), EStatictisXTypeUtils.GetValue(_xType));

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
                if (trackingDayDict.ContainsKey(datetime))
                {
                    accessNum = trackingDayDict[datetime];
                }
                _userNumHashtable.Add(_count - i, accessNum);
                if (accessNum > _maxUserNum)
                {
                    _maxUserNum = accessNum;
                }
            }

            for (var i = 1; i <= _count; i++)
            {
                LtlArray.Text += $@"
xArray.push('{GetGraphicX(i)}');
yArray.push('{GetGraphicY(i)}');
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
                    _pageUrl = PageUtils.GetAnalysisUrl(nameof(PageAnalysisUser), new NameValueCollection
                    {
                        {"DateFrom",  TbDateFrom.Text},
                        {"DateTo",  TbDateTo.Text},
                        {"XType",  DdlXType.SelectedValue}
                    });
                }
                return _pageUrl;
            }
        }
    }
}

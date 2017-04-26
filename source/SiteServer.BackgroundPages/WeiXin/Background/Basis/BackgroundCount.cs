using System;
using System.Web.UI.WebControls;
using System.Text;
using BaiRong.Core;
using SiteServer.WeiXin.Core;
using SiteServer.WeiXin.Model;

namespace SiteServer.WeiXin.BackgroundPages
{
    public class BackgroundCount : BackgroundBasePageWX
    {
        public Literal ltlCategories;
        public Literal ltlData;

        public void Page_Load(object sender, EventArgs e)
        {
            BreadCrumb(AppManager.WeiXin.LeftMenu.ID_Accounts, AppManager.WeiXin.LeftMenu.Function.ID_Chart, string.Empty, AppManager.WeiXin.Permission.WebSite.Chart);

            var categories = new StringBuilder();
            var data = new StringBuilder();

            var dateTime = TranslateUtils.ToDateTime($"{DateTime.Now.Year}-1-1");
            var index = 0;
            for (; dateTime <= DateTime.Now; dateTime = dateTime.AddMonths(1))
            {
                categories.AppendFormat("'{0}', ", dateTime.ToString("yyyy-MM"));

                var amountPaid = DataProviderWX.CountDAO.GetCount(PublishmentSystemID, dateTime.Year, dateTime.Month, ECountType.UserSubscribe);

                var builder1 = new StringBuilder();
                var builder2 = new StringBuilder();
                var dictionary = DataProviderWX.CountDAO.GetDayCount(PublishmentSystemID, dateTime.Year, dateTime.Month, ECountType.UserSubscribe);
                foreach (var value in dictionary)
                {
                    builder1.AppendFormat("'{0}日', ", value.Key);
                    builder2.AppendFormat("{0}, ", value.Value);
                }
                if (builder1.Length > 0) builder1.Length -= 2;
                if (builder2.Length > 0) builder2.Length -= 2;

                data.AppendFormat(@"
{{
    y: {0},
    color: colors[{1}],
    drilldown: {{
        name: '{2}',
        categories: [{3}],
        data: [{4}],
        color: colors[{1}]
    }}
}},
", amountPaid, index++, dateTime.ToString("yyyy-MM"), builder1.ToString(), builder2.ToString());
                //Hashtable hashtable = DataProvider.UrlDAO.GetCountOfActivityHashtable(productID, dateTime.Year, dateTime.Month);

                //series1.AppendFormat("{0}, ", hashtable[0]);
                //series2.AppendFormat("{0}, ", hashtable[1]);
                //series3.AppendFormat("{0}, ", hashtable[2]);
                //series4.AppendFormat("{0}, ", hashtable[3]);
                //series5.AppendFormat("{0}, ", hashtable[4]);
            }

            if (categories.Length > 0) categories.Length -= 2;
            ltlCategories.Text = categories.ToString();

            if (data.Length > 0) data.Length -= 2;
            ltlData.Text = data.ToString();


            //DateTime dateTime = TranslateUtils.ToDateTime("2013-5-1");
            //string productID = base.Request.QueryString["productID"];

            //StringBuilder categories = new StringBuilder();
            //StringBuilder series1 = new StringBuilder();
            //StringBuilder series2 = new StringBuilder();
            //StringBuilder series3 = new StringBuilder();
            //StringBuilder series4 = new StringBuilder();
            //StringBuilder series5 = new StringBuilder();

            //for (; dateTime <= DateTime.Now; dateTime = dateTime.AddMonths(1))
            //{
            //    categories.AppendFormat("'{0}', ", dateTime.ToString("yyyy-MM"));
            //    Hashtable hashtable = DataProvider.UrlDAO.GetCountOfActivityHashtable(productID, dateTime.Year, dateTime.Month);

            //    series1.AppendFormat("{0}, ", hashtable[0]);
            //    series2.AppendFormat("{0}, ", hashtable[1]);
            //    series3.AppendFormat("{0}, ", hashtable[2]);
            //    series4.AppendFormat("{0}, ", hashtable[3]);
            //    series5.AppendFormat("{0}, ", hashtable[4]);
            //}


            //if (categories.Length > 0) categories.Length -= 2;
            //ltlCategories.Text = categories.ToString();

            //if (series1.Length > 0) series1.Length -= 2;
            //ltlSeries1.Text = series1.ToString();

            //if (series2.Length > 0) series2.Length -= 2;
            //ltlSeries2.Text = series2.ToString();

            //if (series3.Length > 0) series3.Length -= 2;
            //ltlSeries3.Text = series3.ToString();

            //if (series4.Length > 0) series4.Length -= 2;
            //ltlSeries4.Text = series4.ToString();

            //if (series5.Length > 0) series5.Length -= 2;
            //ltlSeries5.Text = series5.ToString();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Model.Enumerations;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Core
{
    public static partial class ControlUtils
    {
        public static class KeywordGradeUI
        {
            public static void AddListItems(ListControl listControl)
            {
                if (listControl != null)
                {
                    var item = new ListItem(EKeywordGradeUtils.GetText(EKeywordGrade.Normal), EKeywordGradeUtils.GetValue(EKeywordGrade.Normal));
                    listControl.Items.Add(item);
                    item = new ListItem(EKeywordGradeUtils.GetText(EKeywordGrade.Sensitive), EKeywordGradeUtils.GetValue(EKeywordGrade.Sensitive));
                    listControl.Items.Add(item);
                    item = new ListItem(EKeywordGradeUtils.GetText(EKeywordGrade.Dangerous), EKeywordGradeUtils.GetValue(EKeywordGrade.Dangerous));
                    listControl.Items.Add(item);
                }
            }
        }
    }
}